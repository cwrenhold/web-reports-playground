package utils

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
