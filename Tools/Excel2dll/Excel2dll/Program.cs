using Excel2dll.Type;
using Excel2dllCore;
using Excel2dllCore.Load;

namespace Excel2dll
{
    class Program
    {
        static int Main(string[] args)
        {
            ManulRegister();
            return WorkFlow.Run(args);
        }

        static void ManulRegister()
        {
            RegisterType.Register("cfgvector2", new TpCFGVector2());
            RegisterType.Register("cfgvector3", new TpCFGVector3());
            RegisterType.Register("cfgvector3[]", new TpCFGVector3Array());
            RegisterType.Register("time", new TpCFGTime());
            RegisterType.Register("distance", new TpCFGDistance());
        }
    }
}
