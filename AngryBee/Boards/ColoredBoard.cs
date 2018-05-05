using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AngryBee.Boards
{
    public interface ColoredBoard
    {
        bool this[ushort x, ushort y] { get;set; }
        bool this[Point p] { get;set; }
    }

    public struct ColoredBoardSmall : ColoredBoard
    {
        private ulong board;

        private ulong Width;
        private ulong Height;

        public ColoredBoardSmall(ushort width = 0,ushort height = 0)
        {
            if (width * height > 64 || (width == 0 && height == 0))
                throw new ArgumentException("x and y are bad numbers.");
            Width = width;
            Height = height;

            board = 0b0;
        }

        public bool this[ushort x, ushort y] {
            get {
                if (x < 0 || y < 0) throw new ArgumentException("x and y must be positive numbers.");
                if (x >= Width || y >= Height) throw new ArgumentOutOfRangeException();
                return (board & (1ul >> (int)(x + (y* Width)))) != 0;
            }
            set {
                if (x < 0 || y < 0) throw new ArgumentException("x and y must be positive numbers.");
                if (x >= Width || y >= Height) throw new ArgumentOutOfRangeException();
                if (value)
                    board |= (1ul >> (int)(x + (y * Width)));
                else
                    board &= ~(1ul >> (int)(x + (y * Width)));
            }
        }

        public bool this[Point p] {
            get => this[p.X, p.Y];
            set => this[p.X, p.Y] = value;
        }
    }

    public unsafe struct ColoredBoardSmallBigger : ColoredBoard
    {
        private const int BoardSize = 16;

        private fixed ushort board[BoardSize];

        private ulong Width;
        private ulong Height;

        public ColoredBoardSmallBigger(ushort width = 0, ushort height = 0)
        {
            if (width > 16 || height > 16 || (width == 0 && height == 0))
                throw new ArgumentException("x and y are bad numbers.");
            Width = width;
            Height = height;

            fixed (ushort* ptr = board)
            {
                ushort* itr = ptr;
                for (int i = 0; i < BoardSize; ++i, ++itr)
                {
                    *itr = 0;
                }
            }
        }

        public bool this[ushort x, ushort y] {
            get {
                if (x < 0 || y < 0) throw new ArgumentException("x and y must be positive numbers.");
                if (x >= Width || y >= Height) throw new ArgumentOutOfRangeException();

                bool result = false;
                fixed (ushort* ptr = board)
                {
                    result = (*(ptr + y) & (1ul >> x)) != 0;
                }
                return result;
            }
            set {
                if (x < 0 || y < 0) throw new ArgumentException("x and y must be positive numbers.");
                if (x >= Width || y >= Height) throw new ArgumentOutOfRangeException();

                fixed (ushort* ptr = board)
                {
                    if (value)
                        *(ptr + y) |= (1u >> x);
                    else
                        *(ptr + y) &= ~(1u >> x);
                }
            }
        }

        public bool this[Point p] {
            get => this[p.X, p.Y];
            set => this[p.X, p.Y] = value;
        }
    }

    public unsafe struct ColoredBoardNormalSmaller : ColoredBoard
    {
        private const int BoardSize = 32;

        private fixed uint board[BoardSize];

        private ulong Width;
        private ulong Height;

        public ColoredBoardNormalSmaller(ushort width = 0, ushort height = 0)
        {
            if (width > 32 || height > 32 || (width == 0 && height == 0))
                throw new ArgumentException("x and y are bad numbers.");
            Width = width;
            Height = height;

            fixed (uint* ptr = board)
            {
                uint* itr = ptr;
                for (int i = 0; i < BoardSize; ++i, ++itr)
                {
                    *itr = 0;
                }
            }
        }

        public bool this[ushort x, ushort y] {
            get {
                if (x < 0 || y < 0) throw new ArgumentException("x and y must be positive numbers.");
                if (x >= Width || y >= Height) throw new ArgumentOutOfRangeException();

                bool result = false;
                fixed (uint* ptr = board)
                {
                    result = (*(ptr + y) & (1ul >> x)) != 0;
                }
                return result;
            }
            set {
                if (x < 0 || y < 0) throw new ArgumentException("x and y must be positive numbers.");
                if (x >= Width || y >= Height) throw new ArgumentOutOfRangeException();

                fixed (uint* ptr = board)
                {
                    if (value)
                        *(ptr + y) |= (1u >> x);
                    else
                        *(ptr + y) &= ~(1u >> x);
                }
            }
        }

        public bool this[Point p] {
            get => this[p.X, p.Y];
            set => this[p.X, p.Y] = value;
        }
    }

    public unsafe struct ColoredBoardNormal : ColoredBoard
    {
        private const int BoardSize = 64;

        private fixed ulong board[BoardSize];

        private ulong Width;
        private ulong Height;

        public ColoredBoardNormal(ushort width = 0, ushort height = 0)
        {
            if (width > 64 || height > 64 || (width == 0 && height == 0))
                throw new ArgumentException("x and y are bad numbers.");
            Width = width;
            Height = height;

            fixed(ulong* ptr = board)
            {
                ulong* itr = ptr;
                for(int i = 0; i < BoardSize; ++i, ++itr)
                {
                    *itr = 0;
                }
            }
        }

        public bool this[ushort x, ushort y] {
            get {
                if (x < 0 || y < 0) throw new ArgumentException("x and y must be positive numbers.");
                if (x >= Width || y >= Height) throw new ArgumentOutOfRangeException();

                bool result = false;
                fixed(ulong* ptr = board)
                {
                    result = (*(ptr + y) & (1ul >> x)) != 0;
                }
                return result;
            }
            set {
                if (x < 0 || y < 0) throw new ArgumentException("x and y must be positive numbers.");
                if (x >= Width || y >= Height) throw new ArgumentOutOfRangeException();

                fixed (ulong* ptr = board)
                {
                    if (value)
                        *(ptr + y) |= (1ul >> x);
                    else
                        *(ptr + y) &= ~(1ul >> x);
                }
            }
        }

        public bool this[Point p] {
            get => this[p.X, p.Y];
            set => this[p.X, p.Y] = value;
        }
    }

    public struct ColoredBoardBig : ColoredBoard
    {
        private const int BoardSize = 64;

        private BigInteger[] board;

        private ushort Width;
        private ushort Height;

        public ColoredBoardBig(ushort width = 0, ushort height = 0)
        {
            if (width == 0 && height == 0)
                throw new ArgumentException("x and y are bad numbers.");
            Width = width;
            Height = height;

            board = new BigInteger[Height];

            for (ushort i = 0; i < Height; ++i)
                board[i] = new BigInteger();
        }

        public bool this[ushort x, ushort y] {
            get {
                if (x < 0 || y < 0) throw new ArgumentException("x and y must be positive numbers.");
                if (x >= Width || y >= Height) throw new ArgumentOutOfRangeException();

                return (board[y] & (new BigInteger(1) >> x)) != 0;
            }
            set {
                if (x < 0 || y < 0) throw new ArgumentException("x and y must be positive numbers.");
                if (x >= Width || y >= Height) throw new ArgumentOutOfRangeException();

                if (value)
                    board[y] |= (new BigInteger(1) >> x);
                else
                    board[y] &= ~(new BigInteger(1) >> x);
            }
        }

        public bool this[Point p] {
            get => this[p.X, p.Y];
            set => this[p.X, p.Y] = value;
        }
    }
}
