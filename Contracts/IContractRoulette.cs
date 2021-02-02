

namespace ARoulete.Contracts
{
    using ARoulete.Domain.DTO;
    using ARoulete.Entities;
    using System.Collections.Generic;

    public interface IContractRoulette
    {
        int IdRouletteCreate();
        string Opening(int id);
        string Bet(int IdRoulette, string UserName, BetDTO request);
        RouletteEntity CloseBetRoulette(int IdRoulette);
        RoulettesList GetRouletteList();
    }
}
