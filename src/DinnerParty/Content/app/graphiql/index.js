import React from 'react';
import ReactDOM from 'react-dom';
import { createStore, combineReducers, applyMiddleware, compose } from 'redux';
import ApolloClient from 'apollo-client';
import { ApolloProvider } from 'react-apollo';
import './index.css'
import Nav from './Nav'
import Stats from './Stats'
import GraphiQL from './GraphiQL';

import { selectedQuery } from './reducers'

const client = new ApolloClient()

const store = createStore(
  combineReducers({
    selected: selectedQuery,
    apollo: client.reducer(),
  }),
  {},
  compose(
    applyMiddleware(client.middleware()),
    window.devToolsExtension ? window.devToolsExtension() : f => f,
  )
);

function Index() {
  const routes = [
    {name: 'Stats', component: <Stats/>},
    {name: 'GraphiQL', component: <GraphiQL/>}
  ]
  return (
    <Nav className="nav" routes={routes}/>
  )
}

function Root() {
  return (
    <ApolloProvider store={store} client={client}>
      <Index/>
    </ApolloProvider>
  )
}

ReactDOM.render(<Root/>, document.getElementById('react-app'));
