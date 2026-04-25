namespace AK.Notification.Application.Templates;

public abstract record NotificationTemplateModel(string CustomerName);

public sealed record WelcomeEmailModel(string CustomerName)
    : NotificationTemplateModel(CustomerName);

public sealed record OrderConfirmationModel(
    string CustomerName,
    string OrderNumber,
    decimal TotalAmount,
    IReadOnlyList<string> ItemSummaries)
    : NotificationTemplateModel(CustomerName);

public sealed record OrderConfirmedModel(
    string CustomerName,
    string OrderNumber,
    decimal TotalAmount)
    : NotificationTemplateModel(CustomerName);

public sealed record OrderCancelledModel(
    string CustomerName,
    string OrderNumber,
    string Reason)
    : NotificationTemplateModel(CustomerName);

public sealed record PaymentSucceededModel(
    string CustomerName,
    string OrderNumber,
    decimal Amount,
    string RazorpayPaymentId)
    : NotificationTemplateModel(CustomerName);

public sealed record PaymentFailedModel(
    string CustomerName,
    string OrderNumber,
    string Reason)
    : NotificationTemplateModel(CustomerName);
