const selectedQuery = (state = null, action) => {
  switch (action.type) {
    case 'SET_SELECTED_QUERY':
      return action.query
    default:
      return state
  }
}

export default selectedQuery
