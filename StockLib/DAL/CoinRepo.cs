using StockLib.DAL.Entity;
using StockLib.DAL.Settings;

namespace StockLib.DAL
{
    public interface ICoinRepo : IBaseRepo<Coin>
    {
    }

    public class CoinRepo : BaseRepo<Coin>, ICoinRepo
    {
        public CoinRepo()
        {
        }
    }
}
