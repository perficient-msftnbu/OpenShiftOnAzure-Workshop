import React, { Component } from 'react';

export class FetchCats extends Component {
  static displayName = FetchCats.name;

  constructor (props) {
    super(props);
    this.state = { cats: [], loading: true };

    fetch('api/FetchCats/GetCats')
      .then(response => response.json())
      .then(data => {
        this.setState({ cats: data, loading: false });
      });
  }

  static renderCatsTable (cats) {
    return (
      <table className='table table-striped'>
        <thead>
          <tr>
            <th>Cat</th>
          </tr>
        </thead>
        <tbody>
          {cats.map(cats =>
            <tr key={cats.selectedCat}>
              <td>{cats.selectedCat}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render () {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : FetchCats.renderCatsTable(this.state.cats);

    return (
      <div>
        <h1>Cats</h1>
        <p>This component demonstrates fetching cats from the server.</p>
        {contents}
      </div>
    );
  }
}
