﻿Feature: Booking

@Booking
Scenario: The customer can book a car
	Given I'm connected
	And I insert a start and end date
	And The list of available vehicles appears
	When I select the first car in the list
	Then The car is booked