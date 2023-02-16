using System;

// # Dùng namespace
namespace NewTestNamespace
{
    class UseNamespace
    {
        
    }
}

namespace A
{
    namespace B
    {
        public struct StructBInA { };
    }
}
namespace A.B.C
{
    public struct StructC { };
}
