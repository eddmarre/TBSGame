using System;

public struct GridPosition : IEquatable<GridPosition>
{
    public int _x;
    public int _z;

    public GridPosition(int x, int z)
    {
        _x = x;
        _z = z;
    }

    public override string ToString()
    {
        return $"x: {_x}; z:{_z}";
    }

    public bool Equals(GridPosition other)
    {
        return this == other;
    }

    public override bool Equals(object obj)
    {
        return obj is GridPosition position &&
               _x == position._x &&
               _z == position._z;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_x, _z);
    }

    public static bool operator ==(GridPosition a, GridPosition b)
    {
        return a._x == b._x && a._z == b._z;
    }

    public static bool operator !=(GridPosition a, GridPosition b)
    {
        return !(a == b);
    }


    public static GridPosition operator +(GridPosition a, GridPosition b)
    {
        return new GridPosition(a._x + b._x, a._z + b._z);
    }
    
    public static GridPosition operator -(GridPosition a, GridPosition b)
    {
        return new GridPosition(a._x - b._x, a._z - b._z);
    }
}
