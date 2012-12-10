[PostageApp](http://postageapp.com) for C#
===================================================

This is a library used to integrate .Net apps with PostageApp service.
Personalized, mass email sending can be offloaded to PostageApp via JSON based API.
All documented API functions have been added. See the API Documentation for more information.

### [API Documentation](http://help.postageapp.com/faqs/api) &bull; [PostageApp FAQs](http://help.postageapp.com/faqs) &bull; [PostageApp Help Portal](http://help.postageapp.com)

Installation
------------
 - Copy `bin/PostageApp.dll` to your project's bin folder and add a reference to the library.

Email Usage
-----------
PostageApp for C# is very easy to use. Here's a simple example:

    PostageApp.PostageApp postageApp = new PostageApp.PostageApp('api_key');
    
    Dictionary<string, string> content = new Dictionary<string, string>();
    content.Add("text/plain", 'Example Text Content');    

    postageApp.subject("Example Subject");
    postageApp.to("recipient@example.com");
    postageApp.from("sender@example.com");
    postageApp.message(content);   
	 
    PostageApp.Response response = postageApp.send(); # returns Response object

If you wish to send both html and plain text parts call message function like this:
    
    Dictionary<string, string> content = new Dictionary<string, string>();
    content.Add("text/plain", 'Example Text Content');    
	content.Add("text/html", 'Example HTML Content');
    
You can set headers all in one go:

	Dictionary<string, string> headers = new Dictionary<string, string>();
    headers.Add("subject", 'Example Subject');    
	headers.Add("from", 'sender@example.com');
    
Recipients can be specified in a number of ways. Here's how you define a single recipient:

    postageApp.to("recipient@example.com");

Here's how you define a list of them who can see each others email addresses:

    postageApp.to("recipient1@example.com, recipient2@example.com");

Here's how you define a list of them who can't see each others emails addresses:

    List<string> recipients = new List<string>();
    recipients.Add("recipient1@example.com");
    recipients.Add("recipient2@example.com");
	
Here's how you define a list of them with variables attached:

    Dictionary<string, Dictionary<string, string>> recipients = new Dictionary<string, Dictionary<string, string>>();
    recipients.Add("recipient1@example.com", new Dictionary<string, string>(){ {"variable_1", "value"},{"variable_2", "value"} });
    recipients.Add("recipient2@example.com", new Dictionary<string, string>(){ {"variable_1", "value"},{"variable_2", "value"} });
    
For more information about formatting of recipients, templates and variables please see [documentation](http://help.postageapp.com/faqs)
    
### Recipient Override
Heres's how to override the recipient:

    postageApp.recipient_override = "recipient@example.com";

### Expose JSON Request
Heres's how to show the actual JSON being sent to the API:

    Console.WriteLine("request json: " + postageApp.showRequest());

### Reponse Object
Heres's how to use the response object, see the documentation for what data is returned in each request:

    PostageApp.Response response = postageApp.send(); # returns Response object
	Console.WriteLine("status: " + response.status);
	Console.WriteLine("uid: " + response.uid);

Other Usage
-----------

### Get Account Info
Provides information about the account.
	
	postageApp.get_account_info();
	
### Get Message Receipt
Confirm that message with a particular UID exists
	
	postageApp.get_message_receipt('Example UID');

### Get Message Transmissions
To get data on individual recipients' delivery and open status, you can pass a particular message UID and receive a Response object containing a JSON encoded set of data for each recipient within that message.
	
	postageApp.get_message_transmissions('Example UID');
	
### Get Messages
Gets a list of all message UIDs within your project, for subsequent use in collection statistics or open rates.
	
	postageApp.get_messages();
	
### Get Method List
Get a list of all available api methods.
	
	postageApp.get_method_list();
	
### Get Metrics
Gets data on aggregate delivery and open status for a project, broken down by current hour, current day, current week, current month with the previous of each as a comparable.
	
	postageApp.get_metrics();
	
### Get Project Info
Provides information about the project. e.g. urls, transmissions, users.
	
	postageApp.get_project_info();
