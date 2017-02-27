Feature: Movements2048

Scenario: When_I_Move_Right_And_There_Is_A_Space_On_The_Right_Of_A_Number_It_Is_A_Valid_Move
Given I have a game board set up as
| Column1 | Column2 | Column3 | Column4 |
|         |         |         | 2       |
|         |         | 2       |         |
|         |         |         |         |
|         |         |         |         |  
When I move Right
Then it is a valid move
And the resultant game board is
| Column1 | Column2 | Column3 | Column4 |
|         |         |         | 2       |
|         |         |         | 2       |
|         |         |         |         |
|         |         |         |         |  

Scenario: When_I_Move_Left_And_There_Is_A_Space_On_The_Left_Of_A_Number_It_Is_A_Valid_Move
Given I have a game board set up as
| Column1 | Column2 | Column3 | Column4 |
| 2       |         |         |         |
|         | 2       |         |         |
|         |         |         |         |
|         |         |         |         | 
When I move Left
Then it is a valid move
And the resultant game board is
| Column1 | Column2 | Column3 | Column4 |
| 2       |         |         |         |
| 2       |         |         |         |
|         |         |         |         |
|         |         |         |         |

Scenario: When_I_Move_Up_And_There_Is_A_Space_Above_A_Number_It_Is_A_Valid_Move
Given I have a game board set up as
| Column1 | Column2 | Column3 | Column4 |
| 2       |         |         |         |
|         | 2       |         |         |
|         |         |         |         |
|         |         |         |         | 
When I move Up
Then it is a valid move
And the resultant game board is
| Column1 | Column2 | Column3 | Column4 |
| 2       | 2       |         |         |
|         |         |         |         |
|         |         |         |         |
|         |         |         |         |

Scenario: When_I_Move_Down_And_There_Is_A_Space_Below_A_Number_It_Is_A_Valid_Move
Given I have a game board set up as
| Column1 | Column2 | Column3 | Column4 |
|         |         |         |         |
|         |         |         |         | 
| 2       |         |         |         |
|         | 2       |         |         |
When I move Down
Then it is a valid move
And the resultant game board is
| Column1 | Column2 | Column3 | Column4 |
|         |         |         |         |
|         |         |         |         |
|         |         |         |         |
| 2       | 2       |         |         |

Scenario: When_I_Move_Right_And_There_Is_A_Similar_Number_On_The_Right_Of_A_Number_It_Is_Merged
Given I have a game board set up as
| Column1 | Column2 | Column3 | Column4 |
|         |         |         | 2       |
| 4       |         | 2       |         |
|         | 2       |         |         |
|         | 4       | 4       |         |  
When I move Right
Then it is a valid move
And the resultant game board is
| Column1 | Column2 | Column3 | Column4 |
|         |         |         | 2       |
|         |         | 4       | 2       |
|         |         |         | 2       |
|         |         |         | 8       | 
