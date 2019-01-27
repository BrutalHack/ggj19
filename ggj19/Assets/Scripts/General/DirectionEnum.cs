using System;

namespace BrutalHack.ggj19.General
{
    public enum DirectionEnum
    {
        North,
        South,
        East,
        West
    }

    public static class Extensions
    {
        public static DirectionEnum Left(this DirectionEnum direction)
        {
            switch (direction)
            {
                case DirectionEnum.North:
                    return DirectionEnum.West;
                case DirectionEnum.West:
                    return DirectionEnum.South;
                case DirectionEnum.South:
                    return DirectionEnum.East;
                case DirectionEnum.East:
                    return DirectionEnum.North;
                default: throw new InvalidOperationException("Unknown ENUM Value: " + direction);
            }
        }

        public static DirectionEnum Reverse(this DirectionEnum direction)
        {
            switch (direction)
            {
                case DirectionEnum.North:
                    return DirectionEnum.South;
                case DirectionEnum.South:
                    return DirectionEnum.North;
                case DirectionEnum.West:
                    return DirectionEnum.East;
                case DirectionEnum.East:
                    return DirectionEnum.West;
                default: throw new InvalidOperationException("Unknown ENUM Value: " + direction);
            }
        }
    }
}