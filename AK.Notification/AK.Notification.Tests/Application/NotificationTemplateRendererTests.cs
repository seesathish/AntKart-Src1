using AK.Notification.Application.Templates;
using AK.Notification.Domain.Enums;
using AK.Notification.Infrastructure.Templates;
using FluentAssertions;

namespace AK.Notification.Tests.Application;

public class NotificationTemplateRendererTests
{
    private readonly NotificationTemplateRenderer _renderer = new();

    [Fact]
    public void WelcomeEmail_ContainsCustomerName()
    {
        var model = new WelcomeEmailModel("Alice");
        var content = _renderer.Render(NotificationTemplateType.WelcomeEmail, model);

        content.Subject.Should().Contain("Alice");
        content.Body.Should().Contain("Alice");
    }

    [Fact]
    public void OrderConfirmation_ContainsOrderNumberAndAmount()
    {
        var model = new OrderConfirmationModel("Bob", "ORD-20260101-ABCD1234", 199.99m, new[] { "1x SHIRT-001 @ ₹199.99" });
        var content = _renderer.Render(NotificationTemplateType.OrderConfirmation, model);

        content.Subject.Should().Contain("ORD-20260101-ABCD1234");
        content.Body.Should().Contain("ORD-20260101-ABCD1234");
        content.Body.Should().Contain("199.99");
    }

    [Fact]
    public void OrderConfirmation_ContainsItemSummaries()
    {
        var items = new[] { "2x MEN-SHIR-001 @ ₹500.00", "1x WOM-DRES-001 @ ₹800.00" };
        var model = new OrderConfirmationModel("Carol", "ORD-20260101-XYZ", 1800m, items);
        var content = _renderer.Render(NotificationTemplateType.OrderConfirmation, model);

        content.Body.Should().Contain("MEN-SHIR-001");
        content.Body.Should().Contain("WOM-DRES-001");
    }

    [Fact]
    public void OrderConfirmed_ContainsOrderNumber()
    {
        var model = new OrderConfirmedModel("Dave", "ORD-20260102-CONF", 250m);
        var content = _renderer.Render(NotificationTemplateType.OrderConfirmed, model);

        content.Subject.Should().Contain("ORD-20260102-CONF");
        content.Body.Should().Contain("ORD-20260102-CONF");
    }

    [Fact]
    public void OrderCancelled_ContainsCancellationReason()
    {
        var model = new OrderCancelledModel("Eve", "ORD-20260103-CANC", "Out of stock");
        var content = _renderer.Render(NotificationTemplateType.OrderCancelled, model);

        content.Body.Should().Contain("Out of stock");
        content.Body.Should().Contain("ORD-20260103-CANC");
    }

    [Fact]
    public void PaymentSucceeded_ContainsAmount()
    {
        var model = new PaymentSucceededModel("Frank", "ORD-20260104-PAY", 350.50m, "pay_abc123");
        var content = _renderer.Render(NotificationTemplateType.PaymentSucceeded, model);

        content.Body.Should().Contain("350.50");
        content.Body.Should().Contain("pay_abc123");
    }

    [Fact]
    public void PaymentFailed_ContainsFailureReason()
    {
        var model = new PaymentFailedModel("Grace", "ORD-20260105-FAIL", "Insufficient funds");
        var content = _renderer.Render(NotificationTemplateType.PaymentFailed, model);

        content.Body.Should().Contain("Insufficient funds");
        content.Body.Should().Contain("ORD-20260105-FAIL");
    }

    [Fact]
    public void UnknownTemplateType_ThrowsArgumentException()
    {
        var model = new WelcomeEmailModel("Test");
        var act = () => _renderer.Render((NotificationTemplateType)999, model);

        act.Should().Throw<ArgumentException>();
    }
}
