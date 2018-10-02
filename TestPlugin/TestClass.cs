using System;

namespace TestPlugin
{
    public class TestClass
    {
        public string T1 => "Hello";
        public int T2 => 0xB4D;
        public float T3 => 32.37f;

        public void T4()
        {
            var i2 = 0;
            while (true) if (i2 % 80 == 0) break; else i2 += 4;
        }
    }
}
