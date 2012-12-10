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
content.Add("text/html", 'Example HTML Content');
    postageApp.subject("Example Subject");
    postageApp.to("example@email.com");
    
    postageApp.from("example@email.com");
    postageApp.message(content);    
    PostageApp.Response response = postageApp.send(); # returns JSON response from the server

If you wish to send both html and plain text parts call message function like this:
    
    $this->postageapp->message(array(
      'text/html'   => 'html content',
      'text/plain'  => 'text content'
    ));
    
You can set headers all in one go:

    $this->postageapp->headers(array(
      'subject' => 'Example Subject',
      'from'    => 'sender@example.com'
    ));
    
Recipients can be specified in a number of ways. Here's how you define a list of them with variables attached:

    $this->postageapp->to(array(
      'recipient1@example.com' => array('variable1' => 'value',
                                        'variable2' => 'value'),
      'recipient2@example.com' => array('variable1' => 'value',
                                        'variable2' => 'value')
    ));
    
For more information about formatting of recipients, templates and variables please see [documentation](http://help.postageapp.com/faqs)
    
### Recipient Override
To override the recipient insert your email address in `config/postageapp.php`:

    $config['recipient_override'] = 'you@example.com';

Other Usage
-----------

### Get Account Info
Provides information about the account.
	
	$this->postageapp->get_account_info();
	
### Get Message Receipt
Confirm that message with a particular UID exists
	
	$this->postageapp->get_message_receipt('Example UID');

### Get Message Transmissions
To get data on individual recipients' delivery and open status, you can pass a particular message UID and receive a JSON encoded set of data for each recipient within that message.
	
	$this->postageapp->get_message_transmissions('Example UID');
	
### Get Messages
Gets a list of all message UIDs within your project, for subsequent use in collection statistics or open rates.
	
	$this->postageapp->get_messages();
	
### Get Method List
Get a list of all available api methods.
	
	$this->postageapp->get_method_list();
	
### Get Metrics
Gets data on aggregate delivery and open status for a project, broken down by current hour, current day, current week, current month with the previous of each as a comparable.
	
	$this->postageapp->get_metrics();
	
### Get Project Info
Provides information about the project. e.g. urls, transmissions, users.
	
	$this->postageapp->get_project_info();
