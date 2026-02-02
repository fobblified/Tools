using System;
using System.Runtime.InteropServices;
using System.Text;

class Program
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct CREDENTIAL
    {
        public int Flags;
        public int Type;
        public string TargetName;
        public string Comment;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
        public int CredentialBlobSize;
        public IntPtr CredentialBlob;
        public int Persist;
        public int AttributeCount;
        public IntPtr Attributes;
        public string TargetAlias;
        public string UserName;
    }

    [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern bool CredEnumerate(string filter, int flag,
        out int count, out IntPtr pCredentials);

    [DllImport("advapi32", SetLastError = true)]
    static extern void CredFree(IntPtr buffer);

    static void Main()
    {
        int count;
        IntPtr pCred;

        if (CredEnumerate(null, 0, out count, out pCred))
        {
            for (int i = 0; i < count; i++)
            {
                IntPtr credPtr = Marshal.ReadIntPtr(
                    pCred, i * IntPtr.Size);

                CREDENTIAL cred = (CREDENTIAL)
                    Marshal.PtrToStructure(credPtr, typeof(CREDENTIAL));

                string pass = "";

                if (cred.CredentialBlob != IntPtr.Zero)
                {
                    pass = Marshal.PtrToStringUni(
                        cred.CredentialBlob,
                        cred.CredentialBlobSize / 2);
                }

                Console.WriteLine("[+] " + cred.TargetName);
                Console.WriteLine("    User: " + cred.UserName);
                Console.WriteLine("    Pass: " + pass);
            }

            CredFree(pCred);
        }
    }
}
