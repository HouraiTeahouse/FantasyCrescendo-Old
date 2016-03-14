using System;

public class Serno {
    public bool IsBaka {
        get { return true; }
    }

    public bool IsStrongest {
        get { return true; }
    }

    public int Bus {
        get { throw new NoBusesInGensokyoException("There are no buses in Gensokyo!"); }
    }

    public override string ToString() {
        return "⑨";
    }
}

public class NoBusesInGensokyoException : Exception {
    public NoBusesInGensokyoException(string e) {
    }
}