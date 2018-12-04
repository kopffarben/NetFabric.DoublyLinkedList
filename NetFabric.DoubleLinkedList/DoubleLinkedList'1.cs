using System;
using System.Collections.Generic;

namespace NetFabric
{
    public sealed partial class DoubleLinkedList<T>
    {
        internal Node head;
        internal Node tail;
        int count;
        int version;

        public DoubleLinkedList()
        {
            head = null;
            tail = null;
            count = 0;
            version = 0;
        }

        public DoubleLinkedList(IEnumerable<T> collection) :
            this()
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));

            AddLast(collection);
        }

        public Node First =>
            head;

        public Node Last =>
            tail;

        public int Count =>
            count;

        internal int Version =>
            version;

        public bool IsEmpty =>
            head is null;

        void ValidateNode(Node node)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            if (node.List != this)
                throw new InvalidOperationException();
        }

        public Node AddAfter(Node node, T value)
        {
            ValidateNode(node);
            var result = new Node
            {
                List = this,
                Value = value,
                Next = node.Next,
                Previous = node,
            };
            if (tail == node)
                tail = result;
            count++;
            version++;
            return result;
        }

        public Node AddBefore(Node node, T value)
        {
            ValidateNode(node);
            var result = new Node
            {
                List = this,
                Value = value,
                Next = node,
                Previous = node.Previous,
            };
            if (head == node)
                head = result;
            count++;
            version++;
            return result;
        }

        public Node AddFirst(T value)
        {
            var result = new Node
            {
                List = this,
                Value = value,
                Next = head,
                Previous = null,
            };
            if (IsEmpty)
                tail = result;
            else
                head.Previous = result;
            head = result;
            count++;
            version++;
            return result;
        }

        public void AddFirst(IEnumerable<T> collection)
        {
            Node tempHead = null;
            Node tempTail = null;
            using (var enumerator = collection.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    tempHead = tempTail = new Node
                    {
                        List = this,
                        Value = enumerator.Current,
                        Next = null,
                        Previous = null,
                    };
                    count++;
                    while (enumerator.MoveNext())
                    {
                        var node = new Node
                        {
                            List = this,
                            Value = enumerator.Current,
                            Next = null,
                            Previous = tempTail,
                        };
                        tempTail.Next = node;
                        tempTail = node;
                        count++;
                    }
                }
            }

            if (tempHead is null)
                return;

            if (IsEmpty)
            {
                head = tempHead;
                tail = tempTail;
            }
            else
            {
                head.Previous = tempTail;
                tempTail.Next = head;
                head = tempHead;
            }
            version++;
        }

        public void AddFirst(DoubleLinkedList<T> list, bool reversed)
        {
            Node tempHead = null;
            Node tempTail = null;
            if (reversed && list.count >= 2)
                AssignReversed();
            else
                Assign();

            if (tempHead is null)
                return;

            if (IsEmpty)
            {
                head = tempHead;
                tail = tempTail;
            }
            else
            {
                head.Previous = tempTail;
                tempTail.Next = head;
                head = tempHead;
            }
            count += list.count;
            version++;

            void Assign()
            {
                var current = head;
                if (!(current is null))
                {
                    tempHead = tempTail = new Node
                    {
                        List = this,
                        Value = current.Value,
                        Next = null,
                        Previous = null,
                    };
                    current = current.Next;
                    while (!(current is null))
                    {
                        var node = new Node
                        {
                            List = this,
                            Value = current.Value,
                            Next = null,
                            Previous = tempTail,
                        };
                        tempTail.Next = node;
                        tempTail = node;
                    }
                }
            }

            void AssignReversed()
            {
                Node temp;
                var current = head;
                while (!(current is null))
                {
                    var node = new Node
                    {
                        List = this,
                        Value = current.Value,
                        Next = current.Previous,
                        Previous = current.Next,
                    };

                    temp = current.Next;
                    current.Next = current.Previous;
                    current.Previous = temp;
                    current = temp;
                }
                tempHead = list.tail;
                tempTail = list.head;
            }
        }

        public void AddFirstFrom(DoubleLinkedList<T> list, bool reversed)
        {
            Node tempHead = null;
            Node tempTail = null;
            if (reversed && list.count >= 2)
                AssignReversed();
            else
                Assign();

            if (tempHead is null)
                return;

            if (IsEmpty)
            {
                head = tempHead;
                tail = tempTail;
            }
            else
            {
                head.Previous = tempTail;
                tempTail.Next = head;
                head = tempHead;
            }
            count += list.count;
            version++;
            list.Invalidate();

            void Assign()
            {
                var current = list.head;
                while (!(current is null))
                {
                    current.List = this;

                    current = current.Next;
                }
                tempHead = list.head;
                tempTail = list.tail;
            }

            void AssignReversed()
            {
                var current = list.head;
                if (!(current is null))
                {
                    tempHead = tempTail = new Node
                    {
                        List = this,
                        Value = current.Value,
                        Next = null,
                        Previous = null,
                    };
                    current = current.Next;
                    while (!(current is null))
                    {
                        var node = new Node
                        {
                            List = this,
                            Value = current.Value,
                            Next = tempHead,
                            Previous = null,
                        };
                        tempHead.Previous = node;
                        tempHead = node;
                        current = current.Next;
                    }
                }
            }
        }

        public Node AddLast(T value)
        {
            var result = new Node
            {
                List = this,
                Value = value,
                Next = null,
                Previous = tail,
            };
            if (IsEmpty)
                head = result;
            else
                tail.Next = result;
            tail = result;
            count++;
            version++;
            return result;
        }

        public void AddLast(IEnumerable<T> collection)
        {
            Node tempHead = null;
            Node tempTail = null;
            using (var enumerator = collection.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    tempHead = tempTail = new Node
                    {
                        List = this,
                        Value = enumerator.Current,
                        Next = null,
                        Previous = null,
                    };
                    count++;
                    while (enumerator.MoveNext())
                    {
                        var node = new Node
                        {
                            List = this,
                            Value = enumerator.Current,
                            Next = null,
                            Previous = tempTail,
                        };
                        tempTail.Next = node;
                        tempTail = node;
                        count++;
                    }
                }
            }

            if (tempHead is null)
                return;

            if (IsEmpty)
            {
                head = tempHead;
                tail = tempTail;
            }
            else
            {
                tail.Next = tempHead;
                tempHead.Previous = tail;
                tail = tempTail;
            }
            version++;
        }

        public void AddLast(DoubleLinkedList<T> list)
        {
            Node tempHead = null;
            Node tempTail = null;
            var current = list.head;
            if (!(current is null))
            {
                tempHead = tempTail = new Node
                {
                    List = this,
                    Value = current.Value,
                    Next = null,
                    Previous = null,
                };
                current = current.Next;
                count++;
                while (!(current is null))
                {
                    var node = new Node
                    {
                        List = this,
                        Value = current.Value,
                        Next = null,
                        Previous = tempTail,
                    };
                    tempTail.Next = node;
                    tempTail = node;
                    current = current.Next;
                    count++;
                }
            }

            if (tempHead is null)
                return;

            if (IsEmpty)
            {
                head = tempHead;
                tail = tempTail;
            }
            else
            {
                tail.Next = tempHead;
                tempHead.Previous = tail;
                tail = tempTail;
            }
            version++;
        }

        public void AddLastFrom(DoubleLinkedList<T> list, bool reversed)
        {
            Node tempHead = null;
            Node tempTail = null;
            if (reversed && list.count >= 2)
                AssignReversed();
            else
                Assign();

            if (tempHead is null)
                return;

            if (IsEmpty)
            {
                head = tempHead;
                tail = tempTail;
            }
            else
            {
                tail.Next = tempHead;
                tempHead.Previous = tail;
                tail = tempTail;
            }
            count += list.count;
            version++;
            list.Invalidate();

            void Assign()
            {
                var current = list.head;
                while (!(current is null))
                {
                    current.List = this;

                    current = current.Next;
                }
                tempHead = list.head;
                tempTail = list.tail;
            }

            void AssignReversed()
            {
                var current = list.head;
                if (!(current is null))
                {
                    tempHead = tempTail = new Node
                    {
                        List = this,
                        Value = current.Value,
                        Next = null,
                        Previous = null,
                    };
                    current = current.Next;
                    while (!(current is null))
                    {
                        var node = new Node
                        {
                            List = this,
                            Value = current.Value,
                            Next = tempHead,
                            Previous = null,
                        };
                        tempHead.Previous = node;
                        tempHead = node;
                        current = current.Next;
                    }
                }
            }
        }

        public void Clear()
        {
            var current = head;
            while (!(current is null))
            {
                var temp = current;
                current = current.Next;
                temp.Invalidate();
            }
            Invalidate();
        }

        internal void Invalidate()
        {
            head = null;
            tail = null;
            count = 0;
            version++;
        }

        public Node Find(T value)
        {
            var node = head;
            if (Object.ReferenceEquals(value, null))
            {
                while (!(node is null))
                {
                    if (Object.ReferenceEquals(node.Value, null))
                        return node;

                    node = node.Next;
                }
            }
            else
            {
                var comparer = EqualityComparer<T>.Default;
                while (!(node is null))
                {
                    if (comparer.Equals(node.Value, value))
                        return node;

                    node = node.Next;
                }
            }
            return null;
        }

        public Node FindLast(T value)
        {
            var node = tail;
            if (Object.ReferenceEquals(value, null))
            {
                while (!(node is null))
                {
                    if (Object.ReferenceEquals(node.Value, null))
                        return node;

                    node = node.Previous;
                }
            }
            else
            {
                var comparer = EqualityComparer<T>.Default;
                while (!(node is null))
                {
                    if (comparer.Equals(node.Value, value))
                        return node;

                    node = node.Previous;
                }
            }
            return null;
        }

        public ForwardEnumeration EnumerateForward() =>
            new ForwardEnumeration(this);

        public ReverseEnumeration EnumerateReversed() =>
            new ReverseEnumeration(this);

        public bool Remove(T value)
        {
            var node = Find(value);
            if (node is null)
                return false;

            if (head == node && tail == node)
            {
                head = null;
                tail = null;
            }
            else
            {
                if (head == node)
                {
                    head = node.Next;
                    node.Next.Previous = null;
                }
                else
                {
                    node.Previous.Next = node.Next;
                }

                if (tail == node)
                {
                    tail = node.Previous;
                    node.Previous.Next = null;
                }
                else
                {
                    node.Next.Previous = node.Previous;
                }
            }
            node.Invalidate();
            count--;
            version++;
            return true;
        }

        public bool RemoveLast(T value)
        {
            var node = FindLast(value);
            if (node is null)
                return false;

            if (head == node && tail == node)
            {
                head = null;
                tail = null;
            }
            else
            {
                if (head == node)
                {
                    head = node.Next;
                    node.Next.Previous = null;
                }
                else
                {
                    node.Previous.Next = node.Next;
                }

                if (tail == node)
                {
                    tail = node.Previous;
                    node.Previous.Next = null;
                }
                else
                {
                    node.Next.Previous = node.Previous;
                }
            }
            node.Invalidate();
            count--;
            version++;
            return true;
        }

        public void RemoveFirst()
        {
            if (IsEmpty)
                throw new InvalidOperationException();

            var node = head;
            if (tail == node)
            {
                head = null;
                tail = null;
            }
            else
            {
                head = node.Next;
                head.Previous = null;
            }
            node.Invalidate();
            count--;
            version++;
        }

        public void RemoveLast()
        {
            if (IsEmpty)
                throw new InvalidOperationException();

            var node = tail;
            if (head == node)
            {
                head = null;
                tail = null;
            }
            else
            {
                tail = node.Previous;
                tail.Next = null;
            }
            node.Invalidate();
            count--;
            version++;
        }

        public DoubleLinkedList<T> Clone()
        {
            var list = new DoubleLinkedList<T>
            {
                head = null,
                tail = null,
                count = count,
                version = 0,
            };

            var current = head;
            if (!(current is null))
            {
                list.head = list.tail = new Node
                {
                    List = this,
                    Value = current.Value,
                    Next = null,
                    Previous = null,
                };
                current = current.Next;
                while (!(current is null))
                {
                    var node = new Node
                    {
                        List = this,
                        Value = current.Value,
                        Next = null,
                        Previous = list.tail,
                    };
                    list.tail.Next = node;
                    list.tail = node;
                    current = current.Next;
                }
            }

            return list;
        }

        public DoubleLinkedList<T> Reverse()
        {
            var list = new DoubleLinkedList<T>
            {
                head = null,
                tail = null,
                count = count,
                version = 0,
            };

            var current = head;
            if (!(current is null))
            {
                list.head = list.tail = new Node
                {
                    List = this,
                    Value = current.Value,
                    Next = null,
                    Previous = null,
                };
                current = current.Next;
                while (!(current is null))
                {
                    var node = new Node
                    {
                        List = this,
                        Value = current.Value,
                        Next = list.head,
                        Previous = null,
                    };
                    list.head.Previous = node;
                    list.head = node;
                    current = current.Next;
                }
            }

            return list;
        }

        public void ReverseInPlace()
        {
            if (count < 2)
                return;

            Node temp;
            var current = head;
            while (!(current is null))
            {
                temp = current.Next;
                current.Next = current.Previous;
                current.Previous = temp;
                current = temp;
            }
            temp = head;
            head = tail;
            tail = temp;
            version++;
        }
    }
}