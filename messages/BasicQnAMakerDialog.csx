using System;
using System.Configuration;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Microsoft.Bot.Connector;


// For more information about this template visit http://aka.ms/azurebots-csharp-qnamaker
[Serializable]
public class BasicQnAMakerDialog : QnAMakerDialog
{
    private static TraceWriter _log;
    // Go to https://qnamaker.ai and feed data, train & publish your QnA Knowledgebase.
    public BasicQnAMakerDialog(TraceWriter log) : base(new QnAMakerService(new QnAMakerAttribute(Utils.GetAppSetting("QnASubscriptionKey"), Utils.GetAppSetting("QnAKnowledgebaseId"))))
    {
        _log = log;
    }
    
      protected override Task DefaultWaitNextMessageAsync(IDialogContext context, IMessageActivity message,
            QnAMakerResult result)
        {
            //return base.DefaultWaitNextMessageAsync(context, message, result);
            var user = message?.From?.Name;
            return context.PostAsync(string.Format("Algo mas en que lo pueda ayudar {0}?",user));
        }

        protected override Task RespondFromQnAMakerResultAsync(IDialogContext context, IMessageActivity message,
            QnAMakerResult result)
        {
            if (result.Score <= 0)
            {                
                result.Answer = "No logre encontrar nada en mi sistema, podria redefinir la pregunta por favor?";
            }
            else
                result.Answer = $"Encontre esta informacion : {result.Answer}";


            return context.PostAsync(result.Answer);
            
        }
}