# E-Junkie integration

http://www.e-junkie.com/ej/help.integration.htm

E-junkie can POST transaction data to any server. 1 POST will occur for each Item in a transaction. 

### Example POST from e-junkie (real data):

**NOTE**, this example may not include all possible fields; fields change based on the selected payment processor. 



	POST /purchase/paymentinfo HTTP/1.1

	Pragma: no-cache

	Accept: */*

	Host: imageresizing.net

	Content-type: application/x-www-form-urlencoded

	Content-Length: 1232

	payment_date=02%3A51%3A26+Jul+18%2C+2012+MST&payer_email=angelagube%40photometer.com&address_name=&address_state=&address_country=&address_city=&address_zip=&address_street=&address_country_code=&first_name=Kathrin&last_name=Fretz&payer_id=XH947JE7P6KRJ&residence_country=CH&payer_status=unverified&invoice=wc8dv152xb70684oguh3n7ds0scsg0oc0okko0&address_status=&payer_business_name=&payer_phone=&custom=&mc_currency=USD&business=billing%40imazen.co&mc_gross=249&mc_shipping=0.00&tax=0.00&item_name1=Resizer+3&item_number1=929356%7C%7CR3Bundle1Ent&mc_gross_1=249&quantity1=1&num_cart_items=1&pending_reason=None&txn_id=6MF63406U4306732Y&payment_status=Completed&txn_type=expresscheckout&payment_type=instant&mc_fee=9.02&mailing_list_status=true&client_id=41912&item_name=Resizer+3&item_number=929356%7C%7CR3Bundle1Ent&quantity=1&option_name1=&option_selection1=&option_name2=&option_selection2=&option_name3=&option_selection3=&charset=utf-8&charset_assumed=true&buyer_ip=195.65.234.10&handshake=ff35a320762dcec799d9c0bb9831577c&discount_codes=&from_name=Imazen&from_email=billing%40imazen.co&mailing_list_status=true&client_shipping_method_id=0&item_cart_position=1&sku=R3Bundle1Ent&expiry_hours=0&max_downloads=9&ej_txn_id=14521369


### Breaking this down into list:

	payment_date=02%3A51%3A26+Jul+18%2C+2012+MST
	payer_email=angelagube%40photometer.com
	address_name=
	address_state=
	address_country=
	address_city=
	address_zip=
	address_street=
	address_country_code=
	first_name=Kathrin
	last_name=Fretz
	payer_id=XH947JE7P6KRJ
	residence_country=CH
	payer_status=unverified
	invoice=wc8dv152xb70684oguh3n7ds0scsg0oc0okko0
	address_status=
	payer_business_name=
	payer_phone=
	custom=
	mc_currency=USD
	business=billing%40imazen.co
	mc_gross=249
	mc_shipping=0.00
	tax=0.00
	item_name1=Resizer+3
	item_number1=929356%7C%7CR3Bundle1Ent
	mc_gross_1=249
	quantity1=1
	num_cart_items=1
	pending_reason=None
	txn_id=6MF63406U4306732Y
	payment_status=Completed
	txn_type=expresscheckout
	payment_type=instant
	mc_fee=9.02
	mailing_list_status=true
	client_id=41912
	item_name=Resizer+3
	item_number=929356%7C%7CR3Bundle1Ent
	quantity=1
	option_name1=
	option_selection1=
	option_name2=
	option_selection2=
	option_name3=
	option_selection3=
	charset=utf-8
	charset_assumed=true
	buyer_ip=195.65.234.10
	handshake=ff35a320762dcec799d9c0bb9831577c
	discount_codes=
	from_name=Imazen
	from_email=billing%40imazen.co
	mailing_list_status=true
	client_shipping_method_id=0
	item_cart_position=1
	sku=R3Bundle1Ent
	expiry_hours=0
	max_downloads=9
	ej_txn_id=14521369



## Multi-item transaction example

This is a 2 item transaction


### POST #1 

	residence_country=US&payer_business_name=Foliotek+Inc&first_name=Christopher&last_name=Miller&payer_email=dustins%40foliotek.com&payer_phone=&payer_street=5900-B+North+Tower+Dr.&payer_city=Columbia&payer_state=MO&payer_zip=65202&payer_country_code=US&address_name=+&address_business_name=&address_phone=&address_street=&address_city=&address_state=&address_zip=&address_country_code=US&address_country=US&payment_date=13%3A15%3A35+Jul+09%2C+2012+MST&custom=&mc_currency=USD&business=billing%40imazen.co&mc_gross=399&mc_shipping=0&tax=0&txn_type=ppdirect&payment_type=Instant&invoice=wc8dur8mwza1962rxveo8qsg0okc4cog448gs8&buyer_ip=66.112.97.8&card_last_four=xxxx&card_type=Visa&mailing_list_status=true&charset=utf-8

	&item_name1=Resizer+3&item_number1=929356%7C%7CR3Bundle1Ent&mc_gross_1=199.5&quantity1=1&item_name2=Resizer+3&item_number2=929356%7C%7CR3Bundle2Ent&mc_gross_2=199.5&quantity2=1&num_cart_items=2&txn_id=9N0158558A149201K&payment_status=Completed&pending_reason=&handshake=ff35a320762dcec799d9c0bb9831577c&discount_codes=For+Cart+Item+Total%3A+WRONGLICENSE1&from_name=Imazen&from_email=billing%40imazen.co&mailing_list_status=true&client_shipping_method_id=0&item_cart_position=1&item_number=929356&sku=R3Bundle1Ent&expiry_hours=0&max_downloads=9&ej_txn_id=14420604

### Post #2

	residence_country=US&payer_business_name=Foliotek+Inc&first_name=Christopher&last_name=Miller&payer_email=dustins%40foliotek.com&payer_phone=&payer_street=5900-B+North+Tower+Dr.&payer_city=Columbia&payer_state=MO&payer_zip=65202&payer_country_code=US&address_name=+&address_business_name=&address_phone=&address_street=&address_city=&address_state=&address_zip=&address_country_code=US&address_country=US&payment_date=13%3A15%3A35+Jul+09%2C+2012+MST&custom=&mc_currency=USD&business=billing%40imazen.co&mc_gross=399&mc_shipping=0&tax=0&txn_type=ppdirect&payment_type=Instant&invoice=wc8dur8mwza1962rxveo8qsg0okc4cog448gs8&buyer_ip=66.112.97.8&card_last_four=xxxx &card_type=Visa&mailing_list_status=true&charset=utf-8

	&item_name1=Resizer+3&item_number1=929356%7C%7CR3Bundle1Ent&mc_gross_1=199.5&quantity1=1&item_name2=Resizer+3&item_number2=929356%7C%7CR3Bundle2Ent&mc_gross_2=199.5&quantity2=1&num_cart_items=2&txn_id=9N0158558A149201K&payment_status=Completed&pending_reason=&handshake=ff35a320762dcec799d9c0bb9831577c&discount_codes=For+Cart+Item+Total%3A+WRONGLICENSE1&from_name=Imazen&from_email=billing%40imazen.co&mailing_list_status=true&client_shipping_method_id=0&item_cart_position=2&item_number=929356&sku=R3Bundle2Ent&expiry_hours=0&max_downloads=9&ej_txn_id=14420604

