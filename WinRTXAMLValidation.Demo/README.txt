WinRT XAML Validation - Demo
============================

WinRTXAMLValidation.Demo implements a demo project to showcase the usage of the WinRTXAMLValidation library
for user input validation in C#/XAML for WinRT.

The simple business scenario is placing an auction bid. User input gets validated immediately and by pressing
the "Send bid" button.

Technically, the Model.AuctionBid class is attributed for automatic input validation. You can find the validation
attributes in the Model.ValidationAttributes sub-namespace. Additionally, in AuctionService.SendBidAsync(), 
the implementation of manual validation logic is shown.

MainPage.xaml shows the usage of the ValidationPanel and ValidationSummary controls to bind 
validation messages to the UI. The validation styles are defined in the library's ValidationStyles.xaml resource,
which is added as link into the demo project. For your own project, you can either add this file as link or
include a copy into your project. Feel free to adapt it to your needs.


Input to show data validation:

* New bid / Highest bid < 0
* New bid / Highest bid < 102.95
* New bid in ]102.95, 112.95] and press "Send bid"
* New bid > 10295
* New bid > 102950
* Highest bid < New bid