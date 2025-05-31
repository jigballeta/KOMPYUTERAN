using UnityEngine;

public class CustomerInteract : Interactable
{
    private CustomerAI customer;

    void Start()
    {
        customer = GetComponent<CustomerAI>();
        promptMessage = "Accept Payment";
    }

    public override void BaseInteract()
    {
        if (customer != null && !customer.isPaid)
        {
            int paymentAmount = customer.rentTime * 10; // ₱10 per hour logic
            UIManager.Instance.AddCash(paymentAmount);
            customer.isPaid = true;
            customer.manager.AssignPCToCustomer(customer);
            Debug.Log($"Accepted payment from customer. Amount: ₱{paymentAmount}");
        }
    }
}
