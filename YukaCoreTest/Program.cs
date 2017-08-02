using System;
using System.Collections.Generic;
using System.Text;
using YukaCore;

namespace YukaCoreTest
{
    class Program
    {
        static void Main(string[] args)
        {
             OpeningBook.ComVsCom(new ComputerFoundation(new EvaluateMiddle2(), false, 4, 16), new ComputerFoundation(new EvaluateExpert(), false, 4, 16));       
        }
    }
}
