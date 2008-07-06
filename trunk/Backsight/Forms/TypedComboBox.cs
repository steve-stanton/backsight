using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Backsight.Forms
{
    /// <summary>
    /// A simple combo that lets you work with typed items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>This is kind of experimental, written only because I'm getting
    /// tired working with the plain old ObjectCollection that a basic combo works with.</remarks>
    public partial class TypedComboBox<T> : ComboBox, IEnumerable<T> where T : class
    {
        #region Class data

        // none

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>TypedComboBox</c> with nothing in it.
        /// </summary>
        public TypedComboBox()
        {
            InitializeComponent();
        }

        #endregion

        /// <summary>
        /// Adds an item to this combo
        /// </summary>
        /// <param name="item">The item to add</param>
        public void Add(T item)
        {
            base.Items.Add(item);
        }

        /// <summary>
        /// Adds an array of items to this combo
        /// </summary>
        /// <param name="items">The items to add to the combo</param>
        public void AddRange(T[] items)
        {
            base.Items.AddRange(items);
        }

        /// <summary>
        /// Checks whether the combo contains the specified item
        /// </summary>
        /// <param name="item">The item to look for</param>
        /// <returns>True if the specified item is part of this combo</returns>
        public bool Contains(T item)
        {
            int index = IndexOf(item);
            return (index>=0);
        }

        /// <summary>
        /// Attempts to select an item from this combo
        /// </summary>
        /// <param name="item">The item to select</param>
        /// <returns>The index of the selected item (-1 if the item was not found - in that
        /// case, nothing will be selected at return)</returns>
        public int Select(T item)
        {
            return Select(delegate(T t) { return Object.ReferenceEquals(t, item); });
        }

        /// <summary>
        /// Attempts to select the first item from this combo that satisfies the
        /// supplied predicate.
        /// </summary>
        /// <param name="p">The test that indicates whether the item should be selected or not</param>
        /// <returns>The index of the selected item (-1 if the item was not found - in that
        /// case, nothing will be selected at return)</returns>
        public int Select(Predicate<T> p)
        {
            int index = IndexOf(p);

            if (index < 0)
                base.SelectedItem = null;
            else
                base.SelectedIndex = index;

            return index;
        }

        /// <summary>
        /// Determines the index of the specified item within this combo
        /// </summary>
        /// <param name="item">The item to look for</param>
        /// <returns>The index of the selected item (-1 if the item was not found)</returns>
        public int IndexOf(T item)
        {
            return IndexOf(delegate(T t) { return Object.ReferenceEquals(t, item); });
        }

        /// <summary>
        /// Determines the index of the first item within this combo that satisfies the
        /// supplied predicate.
        /// </summary>
        /// <param name="p">The test that indicates whether an item is relevant or not</param>
        /// <returns>The index of the first item that satisfies the predicate (-1 if the item was not found)</returns>
        public int IndexOf(Predicate<T> p)
        {
            for (int i=0; i<base.Items.Count; i++)
            {
                T t = (T)base.Items[i];
                if (p(t))
                    return i;
            }

            return -1;
        }

        #region IEnumerable<T> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (object o in base.Items)
                yield return (T)o;
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator(); // not this one, the other one
        }

        #endregion
    }
}

