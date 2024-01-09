using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatClient
{
    internal class ChatModel : BindableBase
    {
        private string userName = "";
        public string UserName
        {
            get => userName;
            set => SetProperty(ref userName, value);
        }

        private string message;

        public string Message
        {
            get => message;
            set => SetProperty(ref message, value);

        }

        private IReadOnlyList<string> chat = Array.Empty<string>();
        public IReadOnlyList<string> Chat
        {
            get => chat;
            set => SetProperty(ref chat, value);
        }

    }
}
