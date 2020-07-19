using System;
using System.Collections.Generic;
using System.Text;

namespace DSS_Coding_Challenge
{
    public enum Machine
    {
        Cutter,
        Bender,
        Welder,
        Tester,
        Painter,
        Packer
    }

    static class MachineMethods
    {
        static public String GetString(this Machine t) => t switch
        {
            Machine.Cutter  => "Vágó",
            Machine.Bender  => "Hajlító",
            Machine.Welder  => "Hegesztő",
            Machine.Tester  => "Tesztelő",
            Machine.Painter => "Festő",
            Machine.Packer  => "Csomagoló",
            _ => throw new ArgumentException(message: "Hibás enum érték"),
        };
    }
}
