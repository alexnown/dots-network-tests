namespace com.tictactoe.common
{
    public class TicTacToeUtils
    {
        private static ushort[] WIN_MASKS = { 0b001001001, 0b010010010, 0b100100100, 0b111, 0b111000, 0b111000000, 0b100010001, 0b001010100 };

        public static ushort CheckPlayerWin(ushort cellState)
        {
            foreach(var mask in WIN_MASKS)
            {
                int result = mask & cellState;
                if(result == mask)
                {
                    return mask;
                }
            }
            return 0;
        }

        public static bool CheckIsDraw(ushort playerCells1, ushort playerCells2)
        {
            int freeCells = ~(playerCells1 | playerCells2);
            if(CheckPlayerWin((ushort)(freeCells | playerCells1)) == 0 
                && CheckPlayerWin((ushort)(freeCells | playerCells2)) == 0)
            {
                return true;
            }
            return false;
        }

        public static byte GetIsDrawResultFlags()
        {
            return 0b1000;
        }

        public static byte GetPlayerWinResultFlags(int playerOrder)
        {
            int playerWinMask = 1 << (playerOrder + 1);
            return (byte)(0b1000 | playerWinMask);
        }

        public static bool GameIsEnded(byte gameResultMask)
        {
            return (gameResultMask & 0b1000) > 0;
        }

        public static bool IsDraw(byte gameResultMask)
        {
            return GameIsEnded(gameResultMask) && ((gameResultMask & 0b111) == 0);
        }

        public static bool PlayerIsWin(byte gameResultMask, int playerOrder)
        {
            int playerWinMask = 1 << (playerOrder + 1);
            return (gameResultMask & playerWinMask) > 0;
        }
    }
}