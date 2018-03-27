using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace HotelBot.Dialogs
{
    [Serializable]
    public class GreetingDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Hi I am Amol Bot?");
            await Respond(context);
            context.Wait(MessageReceivedAsync);

            // return Task.CompletedTask;
        }

        private static async Task Respond(IDialogContext context)
        {
            var userName = String.Empty;
            context.UserData.TryGetValue<string>("Name", out userName);
            if(string.IsNullOrEmpty(userName))
            {
                await context.PostAsync("What is your name?");
                context.UserData.SetValue<bool>("GetName", true);
            }
            else
            {
                await context.PostAsync(String.Format("Hi {0}.  How can I help you today?", userName));
            }
        }
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            var userName = String.Empty;
            var getName = false;
            context.UserData.TryGetValue<string>("Name", out userName);
            context.UserData.TryGetValue<bool>("GetName", out getName);

            if (getName)
            {
                userName = message.Text;
                context.UserData.SetValue<string>("Name", userName);
                context.UserData.SetValue<bool>("GetName", false);
            }
            await Respond(context);
            context.Done(message);
        }
        //private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        //{
        //    var userName = string.Empty;
        //    var message = await result;
        //    var activity = await result as Activity;

        //    // calculate something for us to return
        //    var getName = false;
        //    int length = (activity.Text ?? string.Empty).Length;
        //    context.UserData.TryGetValue<string>("Name",out userName);
        //    context.UserData.TryGetValue<bool>("GetName", out getName);
        //    if(getName)
        //    {
        //        userName = message.Text;
        //        context.UserData.SetValue<string>("Name", userName);
        //        context.UserData.SetValue<bool>("GetName", false);
        //    }

        //    if(string.IsNullOrEmpty(userName))
        //    {
        //        await context.PostAsync("what is your name");
        //        userName = ((IMessageActivity)activity).Text;
        //        context.UserData.SetValue<string>("GetName", userName);
        //    }
        //    else
        //    {
        //        await context.PostAsync($"Hi {userName}, how can I help you?");
        //    }

        //    // return our reply to the user


        //    context.Wait(MessageReceivedAsync);
        //}
    }
}