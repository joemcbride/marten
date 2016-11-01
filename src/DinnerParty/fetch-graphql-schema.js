// A script that can pull down the result of the introspection query
// from a running graphql server.

// Dependencies:
// npm i -S isomorphic-fetch graphql-tag graphql apollo-client

// Usage:
// node fetch-graphql-schema [graphql url]

// Example:
// node fetch-graphql-schema https://example.com/graphql > graphql-schema.js

// Using require instead of import so we don't need to transpile for NodeJS 6.x
require('isomorphic-fetch');
const parse = require('graphql-tag/parser').parse;
const introspectionQuery = require('graphql/utilities/introspectionQuery').introspectionQuery;
const ApolloPkg = require('apollo-client');
const {
  createNetworkInterface,
  addTypename,
} = ApolloPkg;
const ApolloClient = ApolloPkg.default;

const GRAPHQL_URL = process.argv.slice(-1)[0];
const query = parse(introspectionQuery);

const graphql = new ApolloClient({
  networkInterface: createNetworkInterface({uri:GRAPHQL_URL}),
  queryTransformer: addTypename,
});

graphql.query({ query }).then((result) => {
  console.log(JSON.stringify(result, null, '  '));
}).catch((err) => console.error(err));
