package utils

func Map[TIn any, TOut any](items []TIn, selector func(TIn) TOut) []TOut {
	result := []TOut{}

	for _, item := range items {
		result = append(result, selector(item))
	}

	return result
}

func Filter[T any](items []T, predicate func(T) bool) []T {
	result := []T{}

	for _, item := range items {
		if predicate(item) {
			result = append(result, item)
		}
	}

	return result
}

func Reduce[T any, TAcc any](items []T, seed TAcc, aggregator func(TAcc, T) TAcc) TAcc {
	result := seed

	for _, item := range items {
		result = aggregator(result, item)
	}

	return result
}

func Find[T any](items []T, predicate func(T) bool) *T {
	for _, item := range items {
		if predicate(item) {
			return &item
		}
	}

	return nil
}

type Grouping[TKey any, TItem any] struct {
	Key   TKey    `json:"key"`
	Items []TItem `json:"items"`
}

func GroupBy[TItem any, TKey comparable](
	items []TItem,
	keySelector func(TItem) TKey) []Grouping[TKey, TItem] {
	result := []Grouping[TKey, TItem]{}

	for _, item := range items {
		key := keySelector(item)
		found := false
		for i, group := range result {
			if group.Key == key {
				result[i].Items = append(result[i].Items, item)
				found = true
				break
			}
		}

		if !found {
			result = append(result, Grouping[TKey, TItem]{Key: key, Items: []TItem{item}})
		}
	}

	return result
}

func GroupByComplex[TItem any, TKey any](
	items []TItem,
	keySelector func(TItem) TKey,
	equalityComparer func(TKey, TKey) bool) []Grouping[TKey, TItem] {
	result := []Grouping[TKey, TItem]{}

	for _, item := range items {
		key := keySelector(item)
		found := false
		for i, group := range result {
			if equalityComparer(group.Key, key) {
				result[i].Items = append(result[i].Items, item)
				found = true
				break
			}
		}

		if !found {
			result = append(result, Grouping[TKey, TItem]{Key: key, Items: []TItem{item}})
		}
	}

	return result
}
