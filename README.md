# blist.net

A memory-balanced List, that more forgiving of inserting elements at index zero.

##overview:

This is my attempt to solve a niche problem, where a list is likely to be enlarged at the beginning, middle and end; but mainly on either end. 
Although this is not a double-queue, its still a list; can still add/remove/replace any element.   
While a linked list is well suited for such a task, it might be prohibitively expensive for very large data-sets; especially byte-lists.

So my solution is to have the real data hover in the middle of the backing array, rather than at beginning as it is with a list. 
This makes it far cheaper to insert elements at index 0. Inserting at index 1 is still rather bad though.  

##results: 

| structure | add first | add middle | add last |
|---|---|---|---|
|List|1955.3ms|861.3ms|**1.9ms**|
|Linked-List|10.7ms|**12.6ms'**|8.6ms|
|B-List|**2.4ms**|856.6ms|2.7ms|

'middle node is cached cached