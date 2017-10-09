Feature: Notification that an item has been taken out
	In order to be aware of the inventory changes
	As a user of the API
	I want a notification to be raised when i take an item out from the inventory


Scenario: Notification that an item has been taken out
	Given an item with the following fields:
	| Label    | ItemType | ExpirationDate |
	| Label1   | TypeA    | 31/12/2020     |
	And the item has been added to the inventory previously
	When I took an item out from the inventory
	Then there is a notification that an item has been taken out
