import React, { useState, useEffect } from 'react';
import Header from '../../../shared/components/Header';
import styles from './MyTickets.module.css';

interface Ticket {
  ticketId: number;
  seat: string;
  passengerName: string;
  departureTime: string;
  routePath: string | null;
}

interface Bill {
  id: number;
  totalAmount: number;
  seatPrice: number;
  suitcasesPrice: number;
  issuedAt: string;
}

interface Suitcase {
  suitcaseId: number;
  allowedWeight: number;
  itemsCount: number;
}

interface TicketWithDetails extends Ticket {
  bill?: Bill;
  suitcases?: Suitcase[];
  status: 'active' | 'boarded' | 'cancelled';
}

const MyTickets: React.FC = () => {
  const [tickets, setTickets] = useState<TicketWithDetails[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [sortBy, setSortBy] = useState<'date' | 'name' | 'status'>('date');
  const [expandedId, setExpandedId] = useState<number | null>(null);
  const [downloadingId, setDownloadingId] = useState<number | null>(null);

  const API_BASE = 'http://localhost:5117/api';

  useEffect(() => {
    fetchTickets();
  }, []);

  const fetchTickets = async () => {
    setLoading(true);
    setError(null);

    try {
      const ticketsResponse = await fetch(`${API_BASE}/Ticket/detailed`);
      if (!ticketsResponse.ok) throw new Error('Nije moguće učitati karte');
      const ticketsData: Ticket[] = await ticketsResponse.json();

      const suitcasesResponse = await fetch(`${API_BASE}/Suitcases`);
      const suitcasesData: Suitcase[] = suitcasesResponse.ok ? await suitcasesResponse.json() : [];

      const ticketsWithDetails: TicketWithDetails[] = await Promise.all(
        ticketsData.map(async (ticket) => {
          const departureTime = new Date(ticket.departureTime);
          const now = new Date();
          const status =
            departureTime.getTime() < now.getTime() ? 'boarded' : 'active';

          // 🔹 FETCH BILL PO TICKET ID
          let bill: Bill | undefined;
          try {
            const billRes = await fetch(
              `${API_BASE}/Bill/by-ticket/${ticket.ticketId}`
            );
            if (billRes.ok) {
              bill = await billRes.json();
            }
          } catch {
            bill = undefined;
          }

          return {
            ...ticket,
            status,
            bill,
            suitcases: suitcasesData.filter(
              (s) => s.suitcaseId === ticket.ticketId
            ),
          };
        })
      );

      setTickets(ticketsWithDetails);
    } catch (err) {
      console.error(err);
      setError('Nije moguće učitati karte.');
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (d: string) =>
    new Date(d).toLocaleDateString('sr-RS');

  const formatTime = (d: string) =>
    new Date(d).toLocaleTimeString('sr-RS', {
      hour: '2-digit',
      minute: '2-digit',
    });

  const toggleExpand = (id: number) =>
    setExpandedId(expandedId === id ? null : id);

  const getSortedTickets = () => {
    const sorted = [...tickets];
    if (sortBy === 'date')
      return sorted.sort(
        (a, b) =>
          new Date(a.departureTime).getTime() -
          new Date(b.departureTime).getTime()
      );
    if (sortBy === 'name')
      return sorted.sort((a, b) =>
        a.passengerName.localeCompare(b.passengerName)
      );
    return sorted.sort((a, b) => a.status.localeCompare(b.status));
  };

  if (loading) {
    return (
      <div className={styles.page}>
        <Header appName="FlyWise" />
        <p>Učitavanje...</p>
      </div>
    );
  }

  const sortedTickets = getSortedTickets();

  return (
    <div className={styles.page}>
      <Header appName="FlyWise" />

      <div className={styles.container}>
        {error && <div className={styles.errorMessage}>{error}</div>}

        <div className={styles.ticketsGrid}>
          {sortedTickets.map((ticket) => {
            const [from = '', to = ''] =
              ticket.routePath?.split(' -> ') ?? [];

            return (
              <div
                key={ticket.ticketId}
                className={styles.ticketCard}
              >
                <div className={styles.route}>
                  <div>
                    <strong>{from}</strong>
                  </div>
                  ✈️
                  <div>
                    <strong>{to}</strong>
                  </div>
                </div>

                <p>Putnik: {ticket.passengerName}</p>
                <p>Seat: {ticket.seat}</p>
                <p>
                  Polazak: {formatDate(ticket.departureTime)}{' '}
                  {formatTime(ticket.departureTime)}
                </p>

                <button
                  onClick={() => toggleExpand(ticket.ticketId)}
                >
                  Više detalja
                </button>

                {expandedId === ticket.ticketId && (
                  <>
                    {ticket.bill && (
                      <div>
                        <p>Cena sedišta: {ticket.bill.seatPrice} RSD</p>
                        <p>Prtljag: {ticket.bill.suitcasesPrice} RSD</p>
                        <strong>
                          Ukupno: {ticket.bill.totalAmount} RSD
                        </strong>
                      </div>
                    )}
                  </>
                )}
              </div>
            );
          })}
        </div>
      </div>
    </div>
  );
};

export default MyTickets;
