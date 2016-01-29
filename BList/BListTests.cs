using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public void given_empty_blist_when_one_element_is_inserted_at_zero_index_then_blist_iterator_should_return_element()
        {
            var count = 1;
            var expected = new int[count].Select(_ => _random.Next()).ToArray();

            var blist = new BList<int>();

            foreach (var item in expected)
                blist.Insert(0,item);

            CollectionAssert.AreEqual(expected, blist);
        }

        [Test]
        public void given_empty_blist_when_elements_are_added_individually_then_blist_count_should_equal_number_added()
        {
            var count = 2 << 10;
            var expected = new int[count].Select(_ => _random.Next()).ToArray();

            var blist = new BList<int>();

            foreach (var item in expected)
                blist.Add(item);

            Assert.That(blist.Count, Is.EqualTo(count));
        }

        [Test]
        public void given_empty_blist_when_elements_are_added_individually_then_blist_indexer_should_return_elements()
        {
            var count = 2 << 10;
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
        public void given_empty_blist_when_elements_are_inserted_at_zero_index_individually_then_blist_iterator_should_return_elements()
        {
            var count = 32;
            var initial = new int[count].Select(_ => _random.Next()).ToArray();
            var expected = initial.Reverse().ToArray();

            var blist = new BList<int>();

            foreach (var item in initial)
                blist.Insert(0, item);

            Assert.That(blist.Count, Is.EqualTo(32));

            CollectionAssert.AreEqual(expected, blist);
        }

        [Test]
        public void given_existing_blist_when_elements_are_replaced_in_middle_then_blist_iterator_should_return_correct_latest_elements()
        {
            
            var initial = new int[1024].Select(_ => _random.Next()).ToArray();
            var replacement = new int[256].Select(_ => _random.Next()).ToArray();
            var expected = initial.Take(256).Concat(replacement).Concat(initial.Skip(512)).ToArray();

            var blist = new BList<int>();

            foreach (var item in initial)
                blist.Add(item);

            for (int i = 0; i < 256; i++)
                blist[256 + i] = replacement[i];

            CollectionAssert.AreEqual(expected, blist);
        }

        [Test]
        public void given_existing_blist_when_elements_are_inserted_in_middle_then_blist_iterator_should_return_correct_latest_elements()
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
        public void given_existing_blist_when_elements_are_inserted_at_index_zero_then_blist_iterator_should_return_correct_latest_elements()
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
        public void given_existing_blist_when_elements_are_removed_by_index_with_zero_index_then_blist_iterator_should_return_correct_elements()
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
        public void given_existing_blist_when_elements_are_removed_by_element_then_blist_iterator_should_return_correct_elements()
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
    }
}
