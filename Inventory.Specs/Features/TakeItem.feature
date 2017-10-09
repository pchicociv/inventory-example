Feature: Take an item from the inventory by Label
	In order to update the inventory
	As a user of the API
	I want to take an item from the inventory


Scenario: Take an item from the inventory by Label
	Given an item with the following fields:
	| Label    | ItemType | ExpirationDate |
	| Label1   | TypeA    | 31/12/2020     |
	And the item has been added to the inventory previously
	When I took an item out from the inventory
	Then the item is no longer in the inventory
