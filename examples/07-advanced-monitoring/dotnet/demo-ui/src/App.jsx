import React, { useState } from 'react';
import axios from 'axios';
import './App.css';

const API_BASE_URL = process.env.REACT_APP_PRICING_SERVICE_URL || 'http://localhost:5001';

function App() {
  const [funds] = useState([
    { id: 'LUX-001', name: 'Luxembourg Equity Fund' },
    { id: 'LUX-002', name: 'European Bond Fund' },
    { id: 'LUX-003', name: 'Global Multi-Asset Fund' },
    { id: 'LUX-004', name: 'Sustainable Growth Fund' },
    { id: 'LUX-005', name: 'Fixed Income Fund' },
  ]);
  
  const [loading, setLoading] = useState(false);
  const [results, setResults] = useState([]);
  const [statistics, setStatistics] = useState({
    total: 0,
    successful: 0,
    failed: 0,
    avgDuration: 0
  });

  const triggerPricing = async (fundId, fundName) => {
    const startTime = Date.now();
    
    try {
      const response = await axios.post(`${API_BASE_URL}/api/pricing/trigger`, {
        fundId,
        fundName
      });
      
      const duration = Date.now() - startTime;
      
      const result = {
        ...response.data,
        duration,
        status: 'success',
        timestamp: new Date().toISOString()
      };
      
      setResults(prev => [result, ...prev].slice(0, 20));
      updateStatistics(result);
      
      return result;
    } catch (err) {
      const duration = Date.now() - startTime;
      
      const result = {
        fundId,
        fundName,
        duration,
        status: 'error',
        error: err.message,
        timestamp: new Date().toISOString()
      };
      
      setResults(prev => [result, ...prev].slice(0, 20));
      updateStatistics(result);
      
      throw err;
    }
  };

  const triggerMonthEnd = async () => {
    setLoading(true);
    setResults([]);
    
    try {
      const promises = funds.map(fund => 
        triggerPricing(fund.id, fund.name).catch(err => ({
          fundId: fund.id,
          error: err.message,
          status: 'error'
        }))
      );
      
      await Promise.all(promises);
    } catch (err) {
      console.error('Batch error:', err);
    } finally {
      setLoading(false);
    }
  };

  const triggerSingle = async (fund) => {
    setLoading(true);
    try {
      await triggerPricing(fund.id, fund.name);
    } catch (err) {
      console.error('Single trigger error:', err);
    } finally {
      setLoading(false);
    }
  };

  const updateStatistics = (result) => {
    setStatistics(prev => {
      const newTotal = prev.total + 1;
      const newSuccessful = prev.successful + (result.status === 'success' ? 1 : 0);
      const newFailed = prev.failed + (result.status === 'error' ? 1 : 0);
      const newAvgDuration = ((prev.avgDuration * prev.total) + result.duration) / newTotal;
      
      return {
        total: newTotal,
        successful: newSuccessful,
        failed: newFailed,
        avgDuration: Math.round(newAvgDuration)
      };
    });
  };

  const clearResults = () => {
    setResults([]);
    setStatistics({
      total: 0,
      successful: 0,
      failed: 0,
      avgDuration: 0
    });
  };

  return (
    <div className="App">
      <header className="app-header">
        <h1>📊 Event-Driven NAV Calculator Demo</h1>
        <p className="subtitle">Real-time demonstration of event-driven architecture with Apache Kafka</p>
      </header>

      <div className="dashboard-links">
        <a href="http://localhost:3000" target="_blank" rel="noopener noreferrer" className="dashboard-link grafana">
          📈 Grafana Dashboard
        </a>
        <a href="http://localhost:16686" target="_blank" rel="noopener noreferrer" className="dashboard-link jaeger">
          🔍 Jaeger Tracing
        </a>
        <a href="http://localhost:9090" target="_blank" rel="noopener noreferrer" className="dashboard-link prometheus">
          📊 Prometheus Metrics
        </a>
      </div>

      <div className="statistics-panel">
        <div className="stat-card">
          <div className="stat-value">{statistics.total}</div>
          <div className="stat-label">Total Requests</div>
        </div>
        <div className="stat-card success">
          <div className="stat-value">{statistics.successful}</div>
          <div className="stat-label">Successful</div>
        </div>
        <div className="stat-card error">
          <div className="stat-value">{statistics.failed}</div>
          <div className="stat-label">Failed</div>
        </div>
        <div className="stat-card">
          <div className="stat-value">{statistics.avgDuration}ms</div>
          <div className="stat-label">Avg Duration</div>
        </div>
      </div>

      <div className="controls-section">
        <h2>Trigger NAV Calculation</h2>
        
        <div className="fund-buttons">
          {funds.map(fund => (
            <button 
              key={fund.id}
              onClick={() => triggerSingle(fund)}
              disabled={loading}
              className="fund-button"
            >
              <span className="fund-id">{fund.id}</span>
              <span className="fund-name">{fund.name}</span>
            </button>
          ))}
        </div>

        <div className="action-buttons">
          <button 
            onClick={triggerMonthEnd}
            disabled={loading}
            className="month-end-button"
          >
            {loading ? '⏳ Processing...' : '🔥 Simulate Month-End (All Funds)'}
          </button>
          
          {results.length > 0 && (
            <button 
              onClick={clearResults}
              disabled={loading}
              className="clear-button"
            >
              🗑️ Clear Results
            </button>
          )}
        </div>
      </div>

      <div className="results-section">
        <h2>Recent Calculations ({results.length})</h2>
        
        {results.length === 0 ? (
          <div className="empty-state">
            <p>No calculations yet. Click a fund above to start.</p>
          </div>
        ) : (
          <div className="results-grid">
            {results.map((result, index) => (
              <div 
                key={`${result.correlationId || result.fundId}-${index}`} 
                className={`result-card ${result.status}`}
              >
                <div className="result-header">
                  <span className="fund-badge">{result.fundId}</span>
                  <span className={`status-badge ${result.status}`}>
                    {result.status === 'success' ? '✅' : '❌'} {result.status}
                  </span>
                </div>
                
                <div className="result-body">
                  {result.status === 'success' ? (
                    <>
                      <div className="result-detail">
                        <span className="label">Fund:</span>
                        <span className="value">{result.fundName}</span>
                      </div>
                      <div className="result-detail">
                        <span className="label">Securities:</span>
                        <span className="value">{result.priceCount} prices</span>
                      </div>
                      <div className="result-detail">
                        <span className="label">Duration:</span>
                        <span className="value">{result.duration}ms</span>
                      </div>
                      <div className="correlation-id">
                        <small>Correlation: {result.correlationId.substring(0, 8)}...</small>
                      </div>
                    </>
                  ) : (
                    <div className="error-message">
                      {result.error}
                    </div>
                  )}
                </div>
                
                <div className="result-footer">
                  <small>{new Date(result.timestamp).toLocaleTimeString()}</small>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      <footer className="app-footer">
        <p>
          Built with C# (.NET 8), Apache Kafka, React | 
          Part of the <strong>Event-Driven Architecture</strong> blog series
        </p>
      </footer>
    </div>
  );
}

export default App;
