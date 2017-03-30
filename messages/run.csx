#r "Newtonsoft.Json"
#load "BasicQnAMakerDialog.csx"

using System;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using System.Collections.Generic;

using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
 

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"Webhook was triggered!");

    // Initialize the azure bot
    using (BotService.Initialize())
    {
        // Deserialize the incoming activity
        string jsonContent = await req.Content.ReadAsStringAsync();
        var activity = JsonConvert.DeserializeObject<Activity>(jsonContent);

        // authenticate incoming request and add activity.ServiceUrl to MicrosoftAppCredentials.TrustedHostNames
        // if request is authenticated
        if (!await BotService.Authenticator.TryAuthenticateAsync(req, new[] { activity }, CancellationToken.None))
        {
            return BotAuthenticator.GenerateUnauthorizedResponse(req);
        }

        if (activity != null)
        {
            // one of these will have an interface and process it
            switch (activity.GetActivityType())
            {
                case ActivityTypes.Message:
                    await Conversation.SendAsync(activity, () => new BasicQnAMakerDialog(log));
                    break;
                case ActivityTypes.ConversationUpdate:
                      log.Error("ActivityTypes.ConversationUpdate");
                    break;
                case ActivityTypes.ContactRelationUpdate:
                  log.Error("ActivityTypes.ContactRelationUpdate");
                break;
                case ActivityTypes.Typing:
                     log.Error("ActivityTypes.Typing");
                break;
                case ActivityTypes.DeleteUserData:
                    log.Error("ActivityTypes.DeleteUserData");
                break;
                case ActivityTypes.Ping:
                    var clientp = new ConnectorClient(new Uri(activity.ServiceUrl));
                    var replyP = activity.CreateReply("Saludos, como puedo ayudarlo?");
                    await clientp.Conversations.ReplyToActivityAsync(replyP);
                break;
                default:
                    log.Error($"Unknown activity type ignored: {activity.GetActivityType()}");
                    break;
            }
        }
        return req.CreateResponse(HttpStatusCode.Accepted);
    }
}