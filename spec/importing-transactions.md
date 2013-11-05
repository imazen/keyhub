# Automatic Transaction importing

1. KeyHub will receive transaction data via HTTP Post similar to a Paypal IPN. Posts can be verified via a secret key. See ejunkie.md for details.

V3 feature: If the HTTP Post contains the purchasing_entity_id and owning_entity_id, then we can go ahead and create the licenses, since whe know what entities to associate them with. 

2. KeyHub should store this data (except for credit card digits), and also create a secret key for each transaction ID. Each item in the transaction should be split into a separate row in Transaction_Items (I.e, no quantity value, as it would complicate relationships)

We can use the sku field and the 'from_email' (vendor.billing_email) field to locate the matching SKU. 


3. Keyhub should e-mail the 'Payer E-mail' field an e-mail containing a link with these secret transaction keys. 

4. The link should permit the user to log in or create an account, after which the transactions contained in the link will be display for the user to claim. 

5. Every logged in user should be shown

1) All unclaimed transaction items with matching secret keys stored in the session cookie.

2) All unclaimed transaction items with payer_email matching the logged in e-mail address (as long as the e-mail address has been verified).

6. When claiming a transaction, the user can choose an existing entity for the purchasing_entity and owning_entity, or they can choose to create a new one, which will be automatically populated with the transaction information. The Owner Name field will be initally populated with the owning_entity.name value, but should be editable until (edit_ownership_duration + issued < now). 

# Reasoning

PayPal provides the e-mail address of the purchaser, but google checkout provides a masked address. Thus, we cannot create licenses automatically for google checkout transactions. 

However, we *can* send them e-mails containing links to keyhub. 

The links can contain a secret transaction key that we generate based on the real transaction ID, and that can be put into a cookie, so that after the user logs in or creates their account, they are able to claim the unclaimed transactions listed in the cookie. 

I've created a spreadsheet with 3 real transaction items: 

https://docs.google.com/spreadsheet/ccc?key=0AgBQf0FS96bPdG5DM0hBTnA1cmd6UXBCc1F4aF9YMHc

We should also allow a logged in user to enter a paypal or google checkout transaction ID to 'resend' the aforementioned e-mail to the purchaser's e-mail. 