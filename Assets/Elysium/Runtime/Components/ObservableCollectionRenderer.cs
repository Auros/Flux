using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Elysium.Templates;
using JetBrains.Annotations;
using UnityEngine;

namespace Elysium
{
    [PublicAPI, ExecuteAlways, RequireComponent(typeof(ObjectInstantiator))]
    public class ObservableCollectionRenderer : MonoBehaviour
    {
        private IList? _list;
        private INotifyCollectionChanged? _collection;
        private readonly List<ItemContext> _itemContexts = new();

        [SerializeField] private Transform? _contentRoot;
        [SerializeField] private GameObject _template = null!;
        [SerializeField] private ObjectInstantiator _objectInstantiator = null!;

        [PublicAPI]
        protected void Start()
        {
            var children = GetComponentsInChildren<ViewModelDefinition>();
            foreach (var child in children)
            {
                if (Application.isPlaying)
                    Destroy(child.gameObject);
                else
                    DestroyImmediate(child.gameObject);
            }
            CreateInitialItems();
        }

        [PublicAPI]
        protected virtual void OnDestroy()
        {
            DestroyAllItems();
        }

        public void SetRenderer(INotifyCollectionChanged? collection)
        {
            if (collection == _collection)
                return;
            
            if (collection is null)
            {
                if (_collection is not null)
                    _collection.CollectionChanged -= CollectionChanged;
                DestroyAllItems();
            }

            _collection = collection;
            _list = collection as IList;

            if (_collection is null)
                return;
            
            _collection.CollectionChanged -= CollectionChanged;
            _collection.CollectionChanged += CollectionChanged;
            CreateInitialItems();
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var itemAdded = e.OldStartingIndex == -1 && e.NewStartingIndex != -1;
            if (itemAdded)
            {
                var ctx = CreateItem(e.NewItems[0]);
                _itemContexts.Insert(e.NewStartingIndex, ctx);
                ctx.GameObject.transform.SetSiblingIndex(e.NewStartingIndex + 1);
                return;
            }

            var itemRemoved = e.OldStartingIndex != -1 && e.NewStartingIndex == -1;
            if (itemRemoved)
            {
                var ctx = _itemContexts[e.OldStartingIndex];
                _itemContexts.Remove(ctx);
                Destroy(ctx.GameObject);
                return;
            }

            var itemReplaced = e.OldStartingIndex != -1 && e.NewStartingIndex != -1;
            if (itemReplaced)
            {
                var newItem = e.NewItems[0];
                var oldIndex = e.OldStartingIndex;
                _itemContexts[oldIndex].ViewModelDefinition.ViewModel = newItem;
                return;
            }

            var cleared = e.OldStartingIndex == -1 && e.NewStartingIndex == -1;
            if (cleared)
                DestroyAllItems();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void CreateInitialItems()
        {
            if (_collection is null || _list is null)
                return;
            
            DestroyAllItems();
            foreach (var item in _list)
            {
                var ctx = CreateItem(item);
                _itemContexts.Add(ctx);
            }
        }
        
        private void DestroyAllItems()
        {
            foreach (var item in _itemContexts)
            {
                if (Application.isPlaying)
                    Destroy(item.GameObject);
                else
                    DestroyImmediate(item.GameObject);
            }
            _itemContexts.Clear();
        }

        private ItemContext CreateItem(object? model)
        {
            var root = _contentRoot != null ? _contentRoot : transform;
            var clone = _objectInstantiator.Instantiate(_template, root);
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation | Justification: Not called often 
            var definition = clone.GetComponent<ViewModelDefinition>();
            if (!definition)
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation | Justification: Not called often 
                definition = clone.AddComponent<ViewModelDefinition>();
            definition.ViewModel = model;

            clone.SetActive(true);
            ItemContext ctx = new(clone, definition);
            return ctx;
        }

        private class ItemContext
        {
            public GameObject GameObject { get; }
            public ViewModelDefinition ViewModelDefinition { get; }
            
            public ItemContext(GameObject gameObject, ViewModelDefinition viewModelDefinition)
            {
                GameObject = gameObject;
                ViewModelDefinition = viewModelDefinition;
            }
        }
    }
}
