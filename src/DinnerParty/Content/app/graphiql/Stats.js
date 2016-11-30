import React from 'react'
import { connect } from 'react-redux'
import { graphql, compose } from 'react-apollo'
import gql from 'graphql-tag'
import moment from 'moment'
import { setSelectedQuery } from './actions'
import './stats.css'

function Field({field}) {
  return (
    <li>
      {field.name}: {field.returnType} ({field.latency}ms)
    </li>
  )
}

function Type({type}) {
  const fields = type.fields.map((field, i) => <Field key={i} field={field}/>)
  return (
    <div className="type">
      {type.name}
      <ul className="field-list">
        {fields}
      </ul>
    </div>
  )
}

function TypeList({types}) {
  const display = types.map((type, i) => {
    return (
      <li key={i}>
        <Type type={type}/>
      </li>
    )
  })
  return (
    <ul className="type-list">
      {display}
    </ul>
  )
}

function Query({query}) {

  if(query === null || query === undefined) {
    return <div/>
  }

  const start = moment(query.start).format('MMMM Do YYYY, h:mm:ss.SS a')

  return (
    <div className="pull-left query">
      <h2>{query.name} <span>{query.duration}ms</span> <span>{start}</span></h2>
      <TypeList types={query.types}/>
    </div>
  )
}

const QueryWithData = connect(
  (state) => ({ query: state.selected })
)(Query)

function QueryListItem({query, active, onClick}) {
  const className = active ? "active" : null;
  return (
    <li className={className} onClick={e => onClick(e, query)}><a href="#">{query.name} {query.duration}ms</a></li>
  )
}

function Stats({ loading, stats, select, selected }) {
  const onClick = (e, query) => {
    e.preventDefault()
    select(query)
  }

  let result = null

  if (loading) {
    result = (
      <div>Loading...</div>
    )
  } else {
    const queries = stats.map((query, i) => {
      const active = query == selected
      return <QueryListItem key={i} query={query} onClick={onClick} active={active}/>
    })

    result = (
      <ul className="query-list">
        {queries}
      </ul>
    )
  }

  return (
    <div className="stats">
      <h2>Stats</h2>
      <div className="pull-left">{result}</div>
      <QueryWithData/>
    </div>
  )
}

const query = gql`
  query stats {
    stats {
      name
      start
      end
      duration
      types {
        name
        fields {
          name
          returnType
          latency
        }
      }
    }
  }`

export default compose(
  graphql(query, {
    options: { },
    props: ({ data: { loading, stats } }) => ({
      loading,
      stats
    })
  }),
  connect(
    (state) => state,
    (dispatch) => {
      return {
        select: (query) => {
          dispatch(setSelectedQuery(query))
        }
      }
    }
  )
)(Stats)
