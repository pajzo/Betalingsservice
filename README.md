# Betalingsservice

Methods to parse and write files for Betalingsservice

## Parse

### 602 Payment info
```csharp
var paymentInfo = ParserClient.GetPaymentInformation(lines);
```

### 603 Mandate info
```csharp
var mandateInfo = ParserClient.GetMandateInformation(lines);
```

### 621 Information
```csharp
var informationEvents = ParserClient.GetInformationEvents(lines);
```