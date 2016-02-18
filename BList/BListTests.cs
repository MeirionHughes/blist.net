using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using NUnit.Framework;

namespace Testing
{
    [TestFixture]
    public class BListTests
    {
        private Random _random = new Random();

        [Test]
        public void given_empty_blist_when_one_element_is_added_then_blist_count_should_equal_one()
        {
            var count = 1;
            var expected = new int[count].Select(_ => _random.Next()).ToArray();

            var blist = new BList<int>();

            foreach (var item in expected)
                blist.Add(item);

            Assert.That(blist.Count, Is.EqualTo(count));
        }

        [Test]
        public void given_empty_blist_when_one_element_is_added_then_blist_indexer_should_return_element()
        {
            var count = 1;
            var expected = new int[count].Select(_ => _random.Next()).ToArray();

            var blist = new BList<int>();

            foreach (var item in expected)
                blist.Add(item);

            Assert.That(expected[0], Is.EqualTo(blist[0]));
        }

        [Test]
        public void given_empty_blist_when_one_element_is_added_then_blist_iterator_should_return_element()
        {
            var count = 1;
            var expected = new int[count].Select(_ => _random.Next()).ToArray();

            var blist = new BList<int>();

            foreach (var item in expected)
                blist.Add(item);

            CollectionAssert.AreEqual(expected, blist);
        }

        [Test]
        public void
            given_empty_blist_when_one_element_is_inserted_at_zero_index_then_blist_iterator_should_return_element()
        {
            var count = 1;
            var expected = new int[count].Select(_ => _random.Next()).ToArray();

            var blist = new BList<int>();

            foreach (var item in expected)
                blist.Insert(0, item);

            CollectionAssert.AreEqual(expected, blist);
        }

        [Test]
        public void given_empty_blist_when_elements_are_added_individually_then_blist_count_should_equal_number_added()
        {
            var count = 1024;
            var expected = new int[count].Select(_ => _random.Next()).ToArray();

            var blist = new BList<int>();

            foreach (var item in expected)
                blist.Add(item);

            Assert.That(blist.Count, Is.EqualTo(count));
        }

        [Test]
        public void given_empty_blist_when_elements_are_added_individually_then_blist_indexer_should_return_elements()
        {
            var count = 1024;
            var expected = new int[count].Select(_ => _random.Next()).ToArray();

            var blist = new BList<int>();

            foreach (var item in expected)
                blist.Add(item);

            for (int i = 0; i < count; i++)
            {
                Assert.That(expected[i], Is.EqualTo(blist[i]));
            }
        }


        [Test]
        public void given_empty_blist_when_elements_are_added_individually_then_blist_iterator_should_return_elements()
        {
            var count = 1024;
            var expected = new int[count].Select(_ => _random.Next()).ToArray();

            var blist = new BList<int>();

            foreach (var item in expected)
                blist.Add(item);

            CollectionAssert.AreEqual(expected, blist);
        }

        [Test]
        public void
            given_empty_blist_when_elements_are_inserted_at_zero_index_individually_then_blist_iterator_should_return_elements
            ()
        {
            var count = 1024;
            var initial = new int[count].Select(_ => _random.Next()).ToArray();
            var expected = initial.Reverse().ToArray();

            var blist = new BList<int>();

            foreach (var item in initial)
                blist.Insert(0, item);

            Assert.That(blist.Count, Is.EqualTo(count));

            CollectionAssert.AreEqual(expected, blist);
        }

        [Test]
        public void
            given_existing_blist_when_elements_are_replaced_in_middle_then_blist_iterator_should_return_correct_latest_elements
            ()
        {

            var initial = new int[1024].Select(_ => _random.Next()).ToArray();
            var replacement = new int[256].Select(_ => _random.Next()).ToArray();
            var expected = initial.Take(256).Concat(replacement).Concat(initial.Skip(512)).ToArray();

            var blist = new BList<int>();

            foreach (var item in initial)
                blist.Add(item);

            CollectionAssert.AreEqual(initial, blist);

            for (int i = 0; i < 256; i++)
                blist[256 + i] = replacement[i];

            CollectionAssert.AreEqual(expected, blist);
        }

        [Test]
        public void
            given_existing_blist_when_elements_are_inserted_in_middle_then_blist_iterator_should_return_correct_latest_elements
            ()
        {
            var initial = new int[768].Select(_ => _random.Next()).ToArray();
            var inserts = new int[256].Select(_ => _random.Next()).ToArray();
            var expected = initial.Take(256).Concat(inserts).Concat(initial.Skip(256)).ToArray();

            var blist = new BList<int>();

            foreach (var item in initial)
                blist.Add(item);

            for (int i = 0; i < 256; i++)
                blist.Insert(256 + i, inserts[i]);

            Assert.That(blist.Count, Is.EqualTo(1024));

            CollectionAssert.AreEqual(expected, blist);
        }

        [Test]
        public void
            given_existing_blist_when_elements_are_inserted_at_index_zero_then_blist_iterator_should_return_correct_latest_elements
            ()
        {
            var initial = new int[512].Select(_ => _random.Next()).ToArray();
            var insertzero = new int[256].Select(_ => _random.Next()).ToArray();
            var expected = insertzero.Reverse().Concat(initial).ToArray();

            var blist = new BList<int>();

            foreach (var item in initial)
                blist.Add(item);

            foreach (var item in insertzero)
                blist.Insert(0, item);

            CollectionAssert.AreEqual(expected, blist);
        }


        [Test]
        public void
            given_existing_blist_when_elements_are_removed_by_index_with_zero_index_then_blist_iterator_should_return_correct_elements
            ()
        {
            var initial = new int[512].Select(_ => _random.Next()).ToArray();
            var expected = initial.Skip(32).ToArray();

            var blist = new BList<int>();

            foreach (var item in initial)
                blist.Add(item);

            for (int i = 0; i < 32; i++)
            {
                blist.RemoveAt(0);
            }

            Assert.That(blist.Count, Is.EqualTo(480));

            CollectionAssert.AreEqual(expected, blist);
        }

        [Test]
        public void
            given_existing_blist_when_elements_are_removed_by_element_then_blist_iterator_should_return_correct_elements
            ()
        {
            var initial = new int[512].Select(_ => _random.Next()).ToArray();
            var expected = initial.Skip(32).ToArray();

            var blist = new BList<int>();

            foreach (var item in initial)
                blist.Add(item);

            for (int i = 0; i < 32; i++)
            {
                blist.Remove(initial[i]);
            }

            Assert.That(blist.Count, Is.EqualTo(480));

            CollectionAssert.AreEqual(expected, blist);
        }


        [Test]
        public void given_empty_blist_when_multiple_elements_added_then_blist_iteractor_should_return_elements()
        {
            var expected = new int[512].Select(_ => _random.Next()).ToArray();

            var blist = new BList<int>();

            blist.AddRange(expected);

            CollectionAssert.AreEqual(expected, blist);
        }

        [Test]
        public void
            given_existing_blist_when_multiple_elements_inserted_at_zero_then_blist_iteractor_should_return_elements()
        {
            var initial = new int[256].Select(_ => _random.Next()).ToArray();
            var expected = initial.Concat(initial).ToArray();

            var blist = new BList<int>();

            blist.AddRange(initial);
            blist.InsertRange(initial, 0);

            CollectionAssert.AreEqual(expected, blist);
        }

        [Test]
        public void given_existing_blist_when_multiple_elements_replaced_then_blist_iteractor_should_return_elements()
        {
            var initial = new int[256].Select(_ => _random.Next()).ToArray();
            var replace = new int[32].Select(_ => _random.Next()).ToArray();
            var expected = initial.Take(32).Concat(replace).Concat(initial.Skip(64)).ToArray();

            var blist = new BList<int>();

            blist.AddRange(initial);
            blist.ReplaceRange(replace, 32);

            CollectionAssert.AreEqual(expected, blist);
        }

        [Test]
        public void
            given_existing_blist_when_multiple_elements_replaced_at_index_zero_then_blist_iteractor_should_return_elements
            ()
        {
            var initial = new int[256].Select(_ => _random.Next()).ToArray();
            var replace = new int[32].Select(_ => _random.Next()).ToArray();
            var expected = replace.Concat(initial.Skip(32)).ToArray();

            var blist = new BList<int>();

            blist.AddRange(initial);
            blist.ReplaceRange(replace, 0);

            CollectionAssert.AreEqual(expected, blist);
        }

        [Test]
        public void given_blist_when_clear_is_called_then_collectionchanged_raised()
        {
            var blist = new BList<int>();

            blist.AddRange(new int[32].Select(_ => _random.Next()).ToArray());

            NotifyCollectionChangedEventArgs args = null;
            blist.CollectionChanged += (_, a) => args = a;

            blist.Clear();

            Assert.That(args, Is.Not.Null);
            Assert.That(args.Action, Is.EqualTo(NotifyCollectionChangedAction.Reset));
        }

        [Test]
        public void given_blist_when_item_removed_then_collectionchanged_raised()
        {
            var blist = new BList<int>();

            var initial = new int[32].Select(_ => _random.Next()).ToArray();
            var expected = initial[16];

            blist.AddRange(initial);

            NotifyCollectionChangedEventArgs args = null;
            blist.CollectionChanged += (_, a) => args = a;

            blist.RemoveAt(16);

            Assert.That(args, Is.Not.Null);
            Assert.That(args.Action, Is.EqualTo(NotifyCollectionChangedAction.Remove));
            Assert.That(args.OldStartingIndex, Is.EqualTo(16));
            Assert.That(args.OldItems[0], Is.EqualTo(expected));
        }

        [Test]
        public void given_blist_when_item_added_then_collectionchanged_raised()
        {
            var blist = new BList<int>();

            var initial = new int[32].Select(_ => _random.Next()).ToArray();
            var expected = _random.Next();

            blist.AddRange(initial);

            NotifyCollectionChangedEventArgs args = null;
            blist.CollectionChanged += (_, a) => args = a;

            blist.Insert(16, expected);

            Assert.That(args, Is.Not.Null);
            Assert.That(args.Action, Is.EqualTo(NotifyCollectionChangedAction.Add));
            Assert.That(args.NewStartingIndex, Is.EqualTo(16));
            Assert.That(args.NewItems[0], Is.EqualTo(expected));
        }

        [Test]
        public void given_blist_when_item_replaced_then_collectionchanged_raised()
        {
            var blist = new BList<int>();

            var index = 10;
            var initial = new int[32].Select(_ => _random.Next()).ToArray();
            var expectedNew = _random.Next();
            var expectedOld = initial[index];

            blist.AddRange(initial);

            NotifyCollectionChangedEventArgs args = null;
            blist.CollectionChanged += (_, a) => args = a;

            blist[index] = expectedNew;

            Assert.That(args, Is.Not.Null);
            Assert.That(args.Action, Is.EqualTo(NotifyCollectionChangedAction.Replace));
            Assert.That(args.NewStartingIndex, Is.EqualTo(10));
            Assert.That(args.OldStartingIndex, Is.EqualTo(10));
            Assert.That(args.NewItems[0], Is.EqualTo(expectedNew));
            Assert.That(args.OldItems[0], Is.EqualTo(expectedOld));
        }

        [Test]
        public void given_blist_with_one_element_when_two_inserted_at_index_zero_then_3rd_element_should_not_be_null()
        {
            var blist = new BList<string>();

            blist.Add("Foo");
            blist.InsertRange(new string[] {"Hello", "World"}, 0);

            Assert.That(blist.Count, Is.EqualTo(3));

            Assert.That(blist[0], Is.EqualTo("Hello"));
            Assert.That(blist[1], Is.EqualTo("World"));
            Assert.That(blist[2], Is.EqualTo("Foo"));
        }
    }
}