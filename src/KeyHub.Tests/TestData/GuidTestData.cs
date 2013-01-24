using System;

namespace KeyHub.Tests.TestData
{
    public static class GuidTestData
    {
        public static Guid Create(int value)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }
    }
}
