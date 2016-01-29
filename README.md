# blist.net

This is my attempt to solve a niche problem, where a list is likely to be enlarged at the beginning, middle and end, but mainly on either end. 
Although this is not a queue, its still a list.  
While a linked list is well suited for such a task, it might be prohibitively expensive for very large data-sets. 

So my solution is to have the "backing" buffer hover in the middle of the capacity, rather than at beginning. 
This makes it far cheaper to insert elements at index 0. Inserting at index 1 is still rather bad though.  

Results: 

| structure | add first | add last |
|---|---|---|
|List|1326.4ms|1.3ms|
|Linked-List|6.9ms|5.0ms|
|B-List|**4.1ms**|**5.6ms**|


