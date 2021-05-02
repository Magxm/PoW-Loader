using System.Threading.Tasks;

namespace PoW_Tool_SheetUtilities.MachineTranslator
{
    class TranslationRequest
    {
        public string[] PreContext;
        public string[] PostContext;
        public string Text;
        public string Standardized;

        public string TranslatedText;

        public TranslationRequest(string text, string[] pre, string[] post, string standardized)
        {
            Text = text;
            PreContext = pre;
            PostContext = post;
            Standardized = standardized;
        }
    }
    interface ITranslator
    {
        bool IsUseable();
        void AddTranslationRequest(TranslationRequest request);
        Task ForceTranslate();
    }
}
