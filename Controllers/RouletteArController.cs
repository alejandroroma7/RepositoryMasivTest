using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ARoulete.Entities;
using ARoulete.Contracts;
using ARoulete.Domain.DTO;

namespace ARoulete.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RouletteArController : Controller
    {

        private readonly IContractRoulette contractRoulette;

        public RouletteArController(IContractRoulette contractRoulette)
        {
            this.contractRoulette = contractRoulette;
        }


        [HttpGet]
        public string Menu()
        {
            return "Roulette Welcome Menu";
        }

        [Route("Create")]
        [HttpGet]
        public int Create()
        {
            return this.contractRoulette.IdRouletteCreate();
        }

        [Route("Opening/{Id}")]
        [HttpGet]
        public string Opening(int id)
        {
            return this.contractRoulette.Opening(id);
        }

        [Route("Bet/{IdRoulette}")]
        [HttpPost]
        public string Bet(int IdRoulette,[FromHeader(Name = "UserId")] string UserName, [FromBody] BetDTO request)
        {
            return this.contractRoulette.Bet(IdRoulette, UserName, request);
        }

        [Route("Close/{IdRoulette}")]
        [HttpPost]
        public RouletteEntity CloseBetRoulette(int IdRoulette)
        {
            return this.contractRoulette.CloseBetRoulette(IdRoulette);
        }

        [Route("GetList")]
        [HttpGet]
        public RoulettesList GetRouletteList()
        {
            return this.contractRoulette.GetRouletteList();
        }
    }
}
