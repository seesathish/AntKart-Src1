using AK.Notification.Application.Templates;
using AK.Notification.Domain.Enums;

namespace AK.Notification.Infrastructure.Templates;

internal sealed class NotificationTemplateRenderer : INotificationTemplateRenderer
{
    public NotificationContent Render(NotificationTemplateType type, NotificationTemplateModel model)
        => type switch
        {
            NotificationTemplateType.WelcomeEmail => RenderWelcomeEmail((WelcomeEmailModel)model),
            NotificationTemplateType.OrderConfirmation => RenderOrderConfirmation((OrderConfirmationModel)model),
            NotificationTemplateType.OrderConfirmed => RenderOrderConfirmed((OrderConfirmedModel)model),
            NotificationTemplateType.OrderCancelled => RenderOrderCancelled((OrderCancelledModel)model),
            NotificationTemplateType.PaymentSucceeded => RenderPaymentSucceeded((PaymentSucceededModel)model),
            NotificationTemplateType.PaymentFailed => RenderPaymentFailed((PaymentFailedModel)model),
            _ => throw new ArgumentException($"Unknown template type: {type}", nameof(type))
        };

    private static NotificationContent RenderWelcomeEmail(WelcomeEmailModel model)
    {
        var subject = $"Welcome to AntKart, {model.CustomerName}!";
        var body = $"Hi {model.CustomerName},\n\nWelcome to AntKart! Your account has been created successfully.\n\nStart shopping at AntKart today.\n\n— The AntKart Team";
        return new NotificationContent(subject, body);
    }

    private static NotificationContent RenderOrderConfirmation(OrderConfirmationModel model)
    {
        var itemLines = string.Join("\n", model.ItemSummaries.Select(s => $"  {s}"));
        var subject = $"Order Confirmation — {model.OrderNumber}";
        var body = $"Hi {model.CustomerName},\n\nThank you for your order! Here are your order details:\n\nOrder Number: {model.OrderNumber}\n\nItems:\n{itemLines}\n\nTotal: ₹{model.TotalAmount:N2}\n\nWe will notify you when your order ships.\n\n— The AntKart Team";
        return new NotificationContent(subject, body);
    }

    private static NotificationContent RenderOrderConfirmed(OrderConfirmedModel model)
    {
        var subject = $"Your Order {model.OrderNumber} Has Been Confirmed";
        var body = $"Hi {model.CustomerName},\n\nGreat news! Your order {model.OrderNumber} has been confirmed and is being processed.\n\nOrder Total: ₹{model.TotalAmount:N2}\n\nYou will receive another update when your order ships.\n\n— The AntKart Team";
        return new NotificationContent(subject, body);
    }

    private static NotificationContent RenderOrderCancelled(OrderCancelledModel model)
    {
        var subject = $"Your Order {model.OrderNumber} Has Been Cancelled";
        var body = $"Hi {model.CustomerName},\n\nYour order {model.OrderNumber} has been cancelled.\n\nReason: {model.Reason}\n\nIf you have any questions, please contact our support team.\n\n— The AntKart Team";
        return new NotificationContent(subject, body);
    }

    private static NotificationContent RenderPaymentSucceeded(PaymentSucceededModel model)
    {
        var subject = $"Payment Confirmed for Order {model.OrderNumber}";
        var body = $"Hi {model.CustomerName},\n\nYour payment has been successfully processed.\n\nOrder Number: {model.OrderNumber}\nAmount Paid: ₹{model.Amount:N2}\nPayment ID: {model.RazorpayPaymentId}\n\nThank you for shopping with AntKart!\n\n— The AntKart Team";
        return new NotificationContent(subject, body);
    }

    private static NotificationContent RenderPaymentFailed(PaymentFailedModel model)
    {
        var subject = $"Payment Failed for Order {model.OrderNumber}";
        var body = $"Hi {model.CustomerName},\n\nUnfortunately, your payment for order {model.OrderNumber} could not be processed.\n\nReason: {model.Reason}\n\nPlease try again or contact our support team for assistance.\n\n— The AntKart Team";
        return new NotificationContent(subject, body);
    }
}
