namespace Betalingsservice.Parser
{
    public enum PaymentTransaction
    {
        AutomatedPaymentCompleted = 236,
        AutomatedPaymentRejected = 237,
        AutomatedPaymentCancelled = 238,
        AutomatedPaymentChargedBack = 239,
        PaymentSlipCompleted = 297,
        PaymentSlipChargedBack = 299
    }
}
