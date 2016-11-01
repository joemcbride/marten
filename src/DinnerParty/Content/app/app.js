import React from 'react'
import ReactDOM from 'react-dom'
import { createStore, combineReducers, applyMiddleware, compose } from 'redux';
import ApolloClient from 'apollo-client';
import { ApolloProvider } from 'react-apollo';
import { reducer as reduxFormReducer } from 'redux-form'

import './app.css'

const client = new ApolloClient()

const store = createStore(
  combineReducers({
    apollo: client.reducer(),
    form: reduxFormReducer
  }),
  {},
  compose(
    applyMiddleware(client.middleware()),
    window.devToolsExtension ? window.devToolsExtension() : f => f,
  )
);

export default function App(Root) {
  ReactDOM.render(
    <ApolloProvider client={client} store={store}>
      <Root/>
    </ApolloProvider>,
    document.getElementById('react-app'))
}
