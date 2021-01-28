using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoW_Tool_SheetUtilities.MachineTranslator
{
    interface ITranslator
    {
        bool IsUseable();
        string Translate(string original);
    }
}
