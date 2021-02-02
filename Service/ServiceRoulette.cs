using ARoulete.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using ARoulete.Entities;
using ARoulete.Domain.DTO;


namespace ARoulete.Service
{
    public class ServiceRoulette : IContractRoulette
    {
        private readonly IDistributedCache distributedCache;

        public ServiceRoulette(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }
        public int IdRouletteCreate()
        {
            int idRoulette = new Random().Next(1, 100);
            var value = new RouletteEntity();
            var newvalue = JsonConvert.SerializeObject(value);
            this.distributedCache.SetString(""+idRoulette, newvalue);
            return idRoulette;
        }

        public string Opening(int id)
        {
            try
            {
                var keyDb = this.distributedCache.GetString(""+id);
                if (keyDb == null)
                {
                    return "Roulette ID entered not exist, must create it first to open";
                }
                else
                {
                    var valueState = JsonConvert.DeserializeObject<RouletteEntity>(keyDb);
                    if ((valueState.State == false) & (valueState.Users.Count>0))
                    {
                        AddtoListRoulettes(valueState.State, id);
                        return "Operation denied";
                    }
                    else
                    {
                        valueState.State = true;                       
                        var newvalue = JsonConvert.SerializeObject(valueState);
                        this.distributedCache.SetString("" + id, newvalue);
                        AddtoListRoulettes(valueState.State, id);
                        return "Success openning roulette number: " + id;
                    }                   
                }               
            }
            catch (StackExchange.Redis.RedisException e)
            {
                return "An exception occurred with redis: " + e;
            }
        }

        public string AddtoListRoulettes(bool state, int id)
        {
            RoulettesList List = new RoulettesList();
            string key= "RoulettesList";
            try
            {
                var listOfRoulletes = this.distributedCache.GetString(key);
                if (listOfRoulletes == null)
                {
                    var value = JsonConvert.SerializeObject(List);
                    this.distributedCache.SetString(key, value);

                    listOfRoulletes = this.distributedCache.GetString(key);
                    var Roulettes = JsonConvert.DeserializeObject<RoulettesList>(listOfRoulletes);
                    Roulettes.List.Add(new ListEntity() { RouletteId = "" + id, State = state });

                    var newvalue = JsonConvert.SerializeObject(Roulettes);
                    this.distributedCache.SetString(key, newvalue);
                }
                else
                {
                    var Roulettes = JsonConvert.DeserializeObject<RoulettesList>(listOfRoulletes);
                    Roulettes.List.Add(new ListEntity() { RouletteId = "" + id, State = state });

                    var newvalue = JsonConvert.SerializeObject(Roulettes);
                    this.distributedCache.SetString(key, newvalue);
                }

                return "List saved successfully";
            }
            catch (StackExchange.Redis.RedisException e)
            {
                return "An exception occurred with redis: " + e;
            }
        }
        public string Bet(int IdRoulette, string UserName, BetDTO request)
        {
            try
            {
                var RouletteIdKey = this.distributedCache.GetString(""+IdRoulette);
                if (RouletteIdKey != null)
                {
                    var rouletteData = JsonConvert.DeserializeObject<RouletteEntity>(RouletteIdKey);
                    if (rouletteData.State == false)
                    {
                        return "The roulette has been closed";
                    }

                    var cash = request.CashToBet;
                    var number = request.BetNumber;
                    var colour = request.Colour;           
                    rouletteData.Users.Add(new UserEntity() { UserName = UserName, CashToBet = cash, BetNumber = number, Colour =colour });

                    var newvalue = JsonConvert.SerializeObject(rouletteData);
                    this.distributedCache.SetString(""+IdRoulette, newvalue);

                    return "The bet has been created!";
                }
                else
                {
                    return "The roulette does not exist, must be created first";
                }
            }
            catch (StackExchange.Redis.RedisException e)
            {
                return "An exception occurred with redis: " + e;
            }
        }

        public RouletteEntity CloseBetRoulette(int IdRoulette)
        {
            RouletteEntity rouletteInfo = new RouletteEntity();
            var RouletteIdKey = this.distributedCache.GetString(""+IdRoulette);
            if (RouletteIdKey != null)
            {
                CloseBetStatus(IdRoulette);
                int winningNumber = new Random().Next(0, 36);
                string winningColour;

                if (winningNumber % 2 != 0)
                {
                    winningColour = "black";
                }
                else
                {
                    winningColour = "red";
                }
                var rouletteData = JsonConvert.DeserializeObject<RouletteEntity>(RouletteIdKey);
                
                foreach (UserEntity user in rouletteData.Users)
                {
                    if ((String.Compare(winningColour, user.Colour) == 0) & user.Colour != null)
                    {
                        user.CashToBet *= 1.8;
                    }
                    else if (user.Colour != null)
                    {                       
                        user.CashToBet = 0;                       
                    }
                    else if ((user.BetNumber == winningNumber) & (String.Compare(winningColour, user.Colour) != 0))
                    {
                        user.CashToBet *= 5;

                    }
                    else
                    {
                        user.CashToBet = 0;
                    }
                }
                rouletteInfo = rouletteData;
                rouletteInfo.State = false;
                var newvalue = JsonConvert.SerializeObject(rouletteData);
                this.distributedCache.SetString("" + IdRoulette, newvalue);


                return rouletteInfo;
            }
            else
            {
                return new RouletteEntity();
            }
        }

        public string CloseBetStatus(int idRoulette)
        {
            try
            {
                string key = "RoulettesList";
                var DataRoulettesDb = this.distributedCache.GetString(key);
                if (DataRoulettesDb != null)
                {
                    var RoulettesData= JsonConvert.DeserializeObject<RoulettesList>(DataRoulettesDb);
                    for (int i = 0; i < RoulettesData.List.Count(); i++)
                    {
                        if (RoulettesData.List[i].RouletteId == ""+idRoulette)
                        {
                            RoulettesData.List[i].State = false;
                        }
                    }

                    var newvalue = JsonConvert.SerializeObject(RoulettesData);
                    this.distributedCache.SetString(key, newvalue);
                }
                else 
                {
                    return "Roulettes list is empty";
                }

                return "Id Roulette is closed now";
            }
            catch (StackExchange.Redis.RedisException e)
            {
                return "An exception occurred with redis: " + e;
            }
        }

        public RoulettesList GetRouletteList()
        {
            string key = "RoulettesList";           
            var RoulettesList = this.distributedCache.GetString(key);
            if (RoulettesList != null)
            {             
                RoulettesList totalRoulletes = JsonConvert.DeserializeObject<RoulettesList>(RoulettesList);                  

                return totalRoulletes;
            }
            else 
            {
                return new RoulettesList();
            }
           
        }
    }
}
