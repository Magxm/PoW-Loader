using System.Threading.Tasks;

namespace PoW_Tool_SheetUtilities.MachineTranslator
{
    class TranslationRequest
    {
        public string[] PreContext;
        public string[] PostContext;
        public string Text;

        public string TranslatedText;

        public TranslationRequest(string text, string[] pre, string[] post)
        {
            Text = text;
            PreContext = pre;
            PostContext = post;
        }
    }
    interface ITranslator
    {
        bool IsUseable();
        void AddTranslationRequest(TranslationRequest request);
        Task ForceTranslate();
    }
}
