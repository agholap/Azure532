using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace HotelBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Hi I am Amol Bot?");
            context.Wait(MessageReceivedAsync);

           // return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var userName = string.Empty;
            var message = await result;
            var activity = await result as Activity;

            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;
            context.UserData.TryGetValue<string>("Name",out userName);
            if(string.IsNullOrEmpty(userName))
            {
                await context.PostAsync("what is your name");
                userName = ((IMessageActivity)activity).Text;
                context.UserData.SetValue<string>("Name", userName);
            }
            else
            {
                await context.PostAsync($"Hi {userName}, how can I help you?");
            }

            // return our reply to the user
            

            context.Wait(MessageReceivedAsync);
        }
    }
}