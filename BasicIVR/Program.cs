using Twilio.TwiML;
using Twilio.AspNet.Core;
using Twilio.TwiML.Messaging;
using Twilio.TwiML.Voice;
using Microsoft.IdentityModel.Tokens;
using Twilio.AspNet.Common;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


app.MapPost("/voice", () => 
{
   var response = new VoiceResponse();
   var gather = new Gather(numDigits:1, action:new Uri("/voice/gather", UriKind.Relative));
            gather
                .Say("Thanks for calling our phone tree. For Sales Press 1. For our hours of operation, press 2. For our address, which will be sent via text, press 3.");
        response.Append(gather);

   return Results.Extensions.TwiML(response);
});

app.MapPost("/voice/gather", ([FromForm] VoiceRequest request)

 => 
{
    var response = new VoiceResponse();
   // If the user entered digits, process their request
		if (!string.IsNullOrEmpty(request.Digits))
		{

            switch (request.Digits)
			{
				case "1":
					response.Say("Thank you. Please hold while your call is transferred.");
		            // Dial to forward to a new number
		            response.Dial("415-123-4567");
                    response.Say("Goodbye");
					break;
				case "2":
					response.Say("Our hours of operation are 8 AM to 5 PM Monday through Friday, 10 AM to 7 PM Saturday, and 10 AM to 3 PM Sunday.");
                    response.Say("Thank you for calling. Goodbye.");
                    response.Hangup();
					break;
				case "3":
                    var message = new Message();
                    message.Body("Here is our address: 375 Beale St #300, San Francisco, CA 94105, USA");
                    response.Say("Our address will be sent to your number via SMS.");
                    response.Say("Thank you for calling. Goodbye.");
                    response.Hangup();
					break;
				default:
					response.Say("Sorry, I don't understand that choice.").Pause();
					response.Redirect(new Uri("/voice", UriKind.Relative));
					break;
			}
		}
		else
		{
			// If no input was sent, redirect to the /voice route
			response.Redirect(new Uri("/voice", UriKind.Relative));
		}
        return Results.Extensions.TwiML(response);
})
.DisableAntiforgery();


app.Run();
